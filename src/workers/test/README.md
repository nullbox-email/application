# send-emails.js

A small Node script to generate inbound email traffic against the Cloudflare Email Routing handler endpoint you’re running locally.

It supports:
- **Realistic sender patterns** (mostly stable sender domain, with optional variation).
- **Per-alias pacing** to avoid concurrent hits to the same alias+routingKey (helps prevent auto-create race conditions when concurrency is high).
- Simple **concurrency** and **global delay** controls.

---

## Requirements

- Node.js **18+** (needs global `fetch`)
- A running local endpoint, e.g. `http://127.0.0.1:8787`

---

## Quick start

```bash
chmod +x send-emails.js
node send-emails.js --iterations 50 --url http://127.0.0.1:8787
````

---

## How it sends requests

For each iteration the script:

1. Chooses an `alias` and `routingKey` (unless fixed by flags).
2. Chooses a `from` sender address using the sender model (stable domain + optional variance).
3. Builds a raw email (headers + HTML body).
4. POSTs to:

`{url}/cdn-cgi/handler/email?from=<from>&to=<to>`

where:

* `to = {alias}@{routingKey}.nullbox.email`

---

## Arguments

### Core traffic shape

#### `--url <string>`

Base URL for your local worker/service.

* Default: `http://127.0.0.1:8787`

Example:

```bash
node send-emails.js --url http://127.0.0.1:8787
```

#### `--iterations <number>`

Total number of emails to send.

* Default: `100`

Example:

```bash
node send-emails.js --iterations 1000
```

#### `--concurrency <number>`

Number of concurrent workers generating traffic.

* Default: `5`

Notes:

* Concurrency increases throughput, but can cause bursts to the same alias unless you also use `--aliasMinGapMs`.

Example:

```bash
node send-emails.js --iterations 500 --concurrency 25
```

#### `--delayMs <number>`

A **global** delay applied after each send attempt within each worker loop.

* Default: `0`

Notes:

* This does **not** guarantee alias-level spacing. For that, use `--aliasMinGapMs`.

Example:

```bash
node send-emails.js --iterations 200 --concurrency 10 --delayMs 50
```

---

## Recipient selection

#### `--alias <string>`

Forces a single alias for all iterations.

* Default: random from `ALIASES`

Example:

```bash
node send-emails.js --iterations 200 --alias github
```

#### `--routingKey <string>`

Forces a single routingKey for all iterations.

* Default: random from `ROUTING_KEYS`

Example:

```bash
node send-emails.js --iterations 200 --routingKey x92qvx
```

---

## Alias pacing (prevents “10 simultaneous emails to the same alias”)

#### `--aliasMinGapMs <number>`

Enforces a minimum time gap between sends to the **same `{alias}|{routingKey}` key** and serializes in-flight sends per key.

* Default: `0` (disabled)

Recommended:

* If you set `--concurrency` above `1`, set `--aliasMinGapMs` to something like `100–500`.

Example:

```bash
node send-emails.js --iterations 500 --concurrency 20 --aliasMinGapMs 250
```

What it does:

* If multiple workers pick the same alias+routingKey, they will queue behind each other.
* After a send completes, the next send for that alias+routingKey waits until the gap time has elapsed.

This is designed to reduce auto-create races in flows like:

* mailbox known, alias missing → auto-create alias → forward

---

## Sender model (realistic “mostly stable domain” traffic)

The script supports three main sender modes:

### Mode A: Fully fixed sender

#### `--sender <email>`

Uses this exact sender email address for all sends.

* Default: not set

Example:

```bash
node send-emails.js --iterations 200 --sender no-reply@amazon.com
```

Behavior:

* If you only pass `--sender`, there is **no variability**.
* If you want variability *even when sender is set*, use `--forceFromVariability true`.

---

### Mode B: Stable domain with varied local parts (recommended)

#### `--fromDomain <domain>`

Uses a stable sender domain (e.g. `amazon.com`) and picks local parts from `--fromLocalParts` (or defaults).

* Default: if omitted, the script derives from `--sender` domain if present; otherwise defaults to `amazon.com`.

Example:

```bash
node send-emails.js --iterations 500 --concurrency 20 --aliasMinGapMs 250 \
  --fromDomain amazon.com
```

#### `--fromLocalParts <comma-separated>`

Comma-separated list of local parts to use with `--fromDomain`.

* Default: `no-reply, notifications, billing, support, alerts, security, updates, team`

Example:

```bash
node send-emails.js --iterations 300 --fromDomain amazon.com \
  --fromLocalParts no-reply,alerts,security
```

---

### Mode C: Stable pool (explicit list)

#### `--stableFromPool <comma-separated>`

Explicit stable sender pool. If provided, the script will pick from this list for “stable” sends.

Example:

```bash
node send-emails.js --iterations 300 \
  --stableFromPool no-reply@amazon.com,alerts@amazon.com,security@amazon.com
```

---

## Sender variability controls (to exercise quarantine / heuristics)

These controls layer on top of the stable sender model.

#### `--fromVariancePct <number>`

Percent of messages that should come from a set of “other legit” senders (different domains).

* Default: `0`

Example (2% of traffic is “other legit”):

```bash
node send-emails.js --iterations 1000 --concurrency 30 --aliasMinGapMs 200 \
  --fromDomain amazon.com --fromVariancePct 2
```

#### `--suspiciousPct <number>`

Percent of messages that should come from an intentionally suspicious sender set (punycode/non-ascii).

* Default: `0`

Purpose:

* Helps trigger the `IsSuspiciousSender(...)` heuristic so messages go to **Quarantine** when not in learning mode / not direct passthrough.

Example (1% suspicious):

```bash
node send-emails.js --iterations 1000 --concurrency 30 --aliasMinGapMs 200 \
  --fromDomain amazon.com --suspiciousPct 1
```

#### `--forceFromVariability true`

If `--sender` is provided, sender variability is normally disabled unless you explicitly configure it.
This flag forces the variability model to apply even when `--sender` is set.

Example:

```bash
node send-emails.js --iterations 300 --sender no-reply@amazon.com \
  --forceFromVariability true --fromVariancePct 2 --suspiciousPct 1
```

---

## Example recipes

### 1) Simple “realistic default” load test

* Moderate concurrency
* Alias-level spacing enabled
* Stable sender domain
* A small amount of legit variation
* A small amount of suspicious traffic (for quarantine)

```bash
node send-emails.js --iterations 2000 --concurrency 25 --aliasMinGapMs 200 \
  --fromDomain amazon.com --fromVariancePct 2 --suspiciousPct 1 \
  --url http://127.0.0.1:8787
```

### 2) Stress alias auto-create without races

* Higher concurrency
* Stronger alias pacing
* Fixed mailbox/routingKey
* Random aliases

```bash
node send-emails.js --iterations 1000 --concurrency 50 --aliasMinGapMs 500 \
  --routingKey x92qvx
```

### 3) Single alias smoke test

* No concurrency (or low)
* Fixed alias + sender

```bash
node send-emails.js --iterations 25 --concurrency 1 \
  --alias github --routingKey x92qvx \
  --sender notifications@github.com
```

### 4) Quarantine-focused run

* Ensure you have learning mode off (or rules not forcing forward)
* Add suspicious traffic

```bash
node send-emails.js --iterations 500 --concurrency 20 --aliasMinGapMs 200 \
  --fromDomain amazon.com --suspiciousPct 10
```

### 5) “Mostly one domain, occasional other legitimate providers”

* No suspicious traffic, only cross-domain legit variation

```bash
node send-emails.js --iterations 1000 --concurrency 30 --aliasMinGapMs 150 \
  --fromDomain amazon.com --fromVariancePct 5
```

---

## Notes on quarantine testing

Your processing flow quarantines when:

* not direct passthrough
* not learning mode
* no explicit enabled sender rule exists yet
* `IsSuspiciousSender(senderEmailNormalized, senderDomainNormalized)` returns true

So a common test approach is:

1. Start with stable “good” senders (mostly forward).
2. Inject a small suspicious percentage (quarantine).
3. Observe:

   * created sender rule behavior
   * quarantine counters (`email.messages.quarantined`)
   * delivery action logs (reason = `SuspiciousSender`)

---

## Troubleshooting

* **`fetch` is not available**

  * Use Node 18+.

* **Still seeing alias auto-create races**

  * Increase `--aliasMinGapMs`.
  * Consider increasing alias variety or routing key variety.
  * Avoid `--alias` fixed with high concurrency unless alias pacing is enabled.

* **Not seeing quarantine**

  * Ensure learning mode is disabled for the alias.
  * Ensure direct passthrough is disabled.
  * Increase `--suspiciousPct`.
  * Verify you don’t already have an enabled sender rule that forces forward.
