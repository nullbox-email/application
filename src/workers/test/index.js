#!/usr/bin/env node
/**
 * node send-emails.js --iterations 100 --url http://127.0.0.1:8787
 *
 * Realism improvements:
 * - Stable sender behavior (same domain, varied local parts) with optional variability.
 * - Per-alias pacing to avoid concurrent hits to the same alias (reduces auto-create races).
 *
 * Examples:
 *   # Mostly stable sender domain with a little variability + suspicious burst testing
 *   node send-emails.js --iterations 500 --concurrency 20 --aliasMinGapMs 250 \
 *     --fromDomain amazon.com --fromVariancePct 2 --suspiciousPct 1
 *
 *   # Fully fixed sender (no variability)
 *   node send-emails.js --iterations 200 --sender no-reply@amazon.com
 *
 *   # Keep current behavior but prevent alias concurrency races
 *   node send-emails.js --iterations 200 --concurrency 10 --aliasMinGapMs 200
 */

const crypto = require("crypto");

function parseArgs(argv) {
  const out = {};
  for (let i = 2; i < argv.length; i++) {
    const a = argv[i];
    if (!a.startsWith("--")) continue;
    const key = a.slice(2);
    const next = argv[i + 1];
    if (!next || next.startsWith("--")) out[key] = true;
    else {
      out[key] = next;
      i++;
    }
  }
  return out;
}

function pick(arr) {
  return arr[Math.floor(Math.random() * arr.length)];
}

function clamp01(n) {
  if (!Number.isFinite(n)) return 0;
  return Math.max(0, Math.min(1, n));
}

function pctToProb(v) {
  if (v === true) return 1;
  const n = Number(v);
  if (!Number.isFinite(n)) return 0;
  return clamp01(n / 100);
}

function rand() {
  return crypto.randomInt(0, 1_000_000) / 1_000_000;
}

function makeMessageId() {
  return `<${crypto.randomBytes(10).toString("hex")}@local.test>`;
}

function nowRfc2822() {
  return new Date().toUTCString();
}

function buildRawEmail({ from, to, subject, html }) {
  return [
    `Received: from smtp.example.com (127.0.0.1)`,
    `        by cloudflare-email.com (unknown) id ${crypto
      .randomBytes(6)
      .toString("base64url")}`,
    `        for <${to}>; ${nowRfc2822()}`,
    `From: "Load Test" <${from}>`,
    `Reply-To: ${from}`,
    `To: ${to}`,
    `Subject: ${subject}`,
    `Content-Type: text/html; charset="utf-8"`,
    `Date: ${nowRfc2822()}`,
    `Message-ID: ${makeMessageId()}`,
    ``,
    html,
  ].join("\n");
}

const ALIASES = [
  "amazon",
  "github",
  "google",
  "stripe",
  "notion",
  "dropbox",
  "slack",
  "xero",
  "zoom",
  "openai",
];

const ROUTING_KEYS = [
  "x92qvx",
  // add more if you want
];

const SUBJECTS = [
  "Testing Email Workers Local Dev",
  "Receipt available",
  "Password reset",
  "Your weekly summary",
  "Invoice ready",
  "Security alert",
  "New login detected",
  "Action required",
];

// Default “other legit” senders for variability mode (non-suspicious)
const LEGIT_VARIATION_SENDERS = [
  "notifications@github.com",
  "billing@stripe.com",
  "support@notion.so",
  "alerts@dropbox.com",
  "security@google.com",
  "updates@slack.com",
  "noreply@zoom.us",
  "team@openai.com",
];

// Intentionally suspicious senders to trigger quarantine heuristic
// (punycode / non-ascii domain or email)
const SUSPICIOUS_SENDERS = [
  // punycode domain
  "security@xn--paypa1-4ve.com",
  // non-ascii in domain (contains Cyrillic 'а' U+0430)
  "support@pаypal.com",
  // non-ascii in local part
  "admín@amazon.com",
];

const DEFAULT_STABLE_LOCAL_PARTS = [
  "no-reply",
  "notifications",
  "billing",
  "support",
  "alerts",
  "security",
  "updates",
  "team",
];

function makeRecipient(alias, routingKey) {
  return `${alias}@${routingKey}.nullbox.email`;
}

/**
 * Build a sender picker that models:
 * - a stable domain (many emails) with varied local parts
 * - optional small percentage of “other legit” senders
 * - optional suspicious percentage to exercise quarantine flow
 */
function makeSenderPicker(args) {
  const fixedSender = typeof args.sender === "string" ? args.sender : null;

  // If fixed sender provided, keep behavior identical unless user *explicitly* asks for variability.
  // (If you want variability even with --sender, pass --forceFromVariability true)
  const forceFromVariability = args.forceFromVariability === true;

  const stableDomain =
    typeof args.fromDomain === "string" && args.fromDomain.trim()
      ? args.fromDomain.trim().toLowerCase()
      : null;

  const stableLocalParts =
    typeof args.fromLocalParts === "string" && args.fromLocalParts.trim()
      ? args.fromLocalParts
          .split(",")
          .map((s) => s.trim())
          .filter(Boolean)
      : DEFAULT_STABLE_LOCAL_PARTS;

  const fromVarianceProb = pctToProb(args.fromVariancePct ?? 0); // % of messages from LEGIT_VARIATION_SENDERS
  const suspiciousProb = pctToProb(args.suspiciousPct ?? 0); // % of messages from SUSPICIOUS_SENDERS

  // If no --fromDomain, fall back to a sensible default stable domain derived from fixed sender if present
  // else default to amazon.com for “stable” behavior.
  const effectiveStableDomain =
    stableDomain ||
    (fixedSender ? String(fixedSender).split("@")[1]?.toLowerCase() : null) ||
    "amazon.com";

  // Also allow a stable "brand" sender list to be user-specified by providing --stableFromPool
  // as comma-separated list; if present, we pick from that list (still stable-ish).
  const stableFromPool =
    typeof args.stableFromPool === "string" && args.stableFromPool.trim()
      ? args.stableFromPool
          .split(",")
          .map((s) => s.trim())
          .filter(Boolean)
      : null;

  function pickStableSender() {
    if (stableFromPool && stableFromPool.length > 0) return pick(stableFromPool);
    const local = pick(stableLocalParts);
    return `${local}@${effectiveStableDomain}`;
  }

  return function pickSender() {
    // Fully fixed sender unless variability is forced or configured
    const variabilityEnabled =
      forceFromVariability || fromVarianceProb > 0 || suspiciousProb > 0 || !!stableDomain;

    if (fixedSender && !variabilityEnabled) return fixedSender;

    const r = rand();

    if (suspiciousProb > 0 && r < suspiciousProb) {
      return pick(SUSPICIOUS_SENDERS);
    }

    if (fromVarianceProb > 0 && r < suspiciousProb + fromVarianceProb) {
      return pick(LEGIT_VARIATION_SENDERS);
    }

    // stable default
    return fixedSender || pickStableSender();
  };
}

/**
 * Per-alias pacing gate:
 * - ensures only one in-flight send per alias+routingKey
 * - enforces a minimum gap between sends for same alias+routingKey
 */
function makePerAliasGate({ aliasMinGapMs }) {
  const gapMs = Math.max(0, Number(aliasMinGapMs ?? 0) || 0);

  // Promise-chain lock per key
  const chains = new Map(); // key -> Promise<void>
  const nextAllowedAt = new Map(); // key -> timestamp ms

  async function acquire(key) {
    const prev = chains.get(key) || Promise.resolve();

    let release;
    const mine = new Promise((resolve) => (release = resolve));
    chains.set(key, prev.then(() => mine));

    // Wait for previous work on this key
    await prev;

    // Enforce min gap
    const now = Date.now();
    const readyAt = nextAllowedAt.get(key) || now;
    const waitMs = Math.max(0, readyAt - now);
    if (waitMs > 0) await sleep(waitMs);

    // Set next allowed time from *now* (after waiting)
    nextAllowedAt.set(key, Date.now() + gapMs);

    return () => {
      release();
      // Optional cleanup: if we're the last link, drop maps.
      // (Hard to know reliably; keep simple.)
    };
  }

  return { acquire };
}

async function sendOne({ baseUrl, from, alias, routingKey, idx }) {
  if (typeof fetch !== "function") {
    throw new Error(
      "Global fetch is not available. Use Node 18+ or switch to the node-fetch import option."
    );
  }

  const to = makeRecipient(alias, routingKey);
  const url =
    `${baseUrl.replace(/\/$/, "")}` +
    `/cdn-cgi/handler/email?from=${encodeURIComponent(from)}&to=${encodeURIComponent(to)}`;

  const subject = `${pick(SUBJECTS)} (#${idx})`;
  const body = buildRawEmail({
    from,
    to,
    subject,
    html: `Hi there<br/>idx=${idx}<br/>alias=${alias}<br/>routingKey=${routingKey}<br/>from=${from}`,
  });

  const res = await fetch(url, {
    method: "POST",
    headers: {
      Accept: "*/*",
      "User-Agent": "local-script",
      "Content-Type": "application/json",
    },
    body,
  });

  const text = await res.text();
  let json = null;
  try {
    json = JSON.parse(text);
  } catch {}

  return {
    ok: res.ok,
    status: res.status,
    alias,
    routingKey,
    from,
    to,
    response: json ?? text,
  };
}

function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

async function main() {
  const args = parseArgs(process.argv);

  const baseUrl = args.url || "http://127.0.0.1:8787";
  const iterations = Number(args.iterations ?? 100);
  const delayMs = Number(args.delayMs ?? 0);

  const fixedAlias = typeof args.alias === "string" ? args.alias : null;
  const fixedRoutingKey = typeof args.routingKey === "string" ? args.routingKey : null;

  const concurrency = Number(args.concurrency ?? 5);

  // New: per-alias pacing (recommended when concurrency > 1)
  // Default to 0 to preserve existing behavior unless you opt in.
  const aliasMinGapMs = Number(args.aliasMinGapMs ?? 0);

  // New: realistic sender picker
  const pickSender = makeSenderPicker(args);

  const gate = makePerAliasGate({ aliasMinGapMs });

  const tasks = Array.from({ length: iterations }, (_, i) => async () => {
    const alias = fixedAlias ?? pick(ALIASES);
    const routingKey = fixedRoutingKey ?? pick(ROUTING_KEYS);

    // Key should match the thing that causes your auto-create race:
    // aliasLocal + mailbox/domain (routingKey identifies mailbox in your addressing scheme)
    const key = `${alias}|${routingKey}`;

    const release = await gate.acquire(key);
    try {
      const from = pickSender();
      return await sendOne({ baseUrl, from, alias, routingKey, idx: i + 1 });
    } finally {
      release();
    }
  });

  const results = [];
  let cursor = 0;

  async function worker() {
    while (cursor < tasks.length) {
      const myIdx = cursor++;
      try {
        const r = await tasks[myIdx]();
        results.push(r);
        console.log(
          `[${r.ok ? "OK" : "FAIL"}] ${String(myIdx + 1).padStart(3, "0")}/${iterations} to=${
            r.to
          } from=${r.from} status=${r.status}`
        );
      } catch (e) {
        results.push({
          ok: false,
          status: 0,
          alias: fixedAlias ?? "(random)",
          routingKey: fixedRoutingKey ?? "(random)",
          from: "(picker)",
          to: "(unknown)",
          response: String(e?.stack || e),
        });
        console.log(
          `[FAIL] ${String(myIdx + 1).padStart(3, "0")}/${iterations} error=${e?.message || e}`
        );
      } finally {
        if (delayMs > 0) await sleep(delayMs);
      }
    }
  }

  await Promise.all(Array.from({ length: Math.max(1, concurrency) }, () => worker()));

  const okCount = results.filter((r) => r.ok).length;
  const failCount = results.length - okCount;
  console.log(`\nDone. ok=${okCount} fail=${failCount}`);

  if (failCount > 0) {
    console.log(
      `First failure sample:\n${JSON.stringify(results.find((r) => !r.ok), null, 2)}`
    );
    process.exitCode = 1;
  }
}

main().catch((e) => {
  console.error(e);
  process.exitCode = 1;
});
