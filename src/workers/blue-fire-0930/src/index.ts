// Cloudflare Email Worker: signed API ingress (HMAC)
// No preamble.

interface Env {
	API_BASE_URL: string;
	EMAIL_HMAC_SECRET: string;

	MAILGUN_API_KEY: string;
	MAILGUN_DOMAIN: string; // e.g. mg.nullbox.email
	MAILGUN_REGION: string; // "us" | "eu"

	// Telemetry
	LOG_LEVEL?: string; // "debug" | "info" | "warn" | "error" | "none"
}

type LogLevel = 'debug' | 'info' | 'warn' | 'error' | 'none';

const LOG_RANK: Record<LogLevel, number> = {
	debug: 10,
	info: 20,
	warn: 30,
	error: 40,
	none: 99,
};

function parseLogLevel(v: unknown): LogLevel {
	const s = (typeof v === 'string' ? v : '').trim().toLowerCase();
	if (s === 'debug' || s === 'info' || s === 'warn' || s === 'error' || s === 'none') return s;
	return 'info';
}

function shouldLog(env: Env, level: LogLevel): boolean {
	const min = parseLogLevel(env.LOG_LEVEL);
	return LOG_RANK[level] >= LOG_RANK[min];
}

/** Structured logs with level + gating */
function log(env: Env, level: LogLevel, event: string, data?: Record<string, unknown>) {
	if (!shouldLog(env, level)) return;

	const payload = {
		ts: new Date().toISOString(),
		level,
		event,
		...(data ?? {}),
	};

	if (level === 'error') console.error(payload);
	else if (level === 'warn') console.warn(payload);
	else console.log(payload);
}

function mailgunBase(region: string) {
	return region?.toLowerCase() === 'eu' ? 'https://api.eu.mailgun.net' : 'https://api.mailgun.net';
}

function safeHeader(v: unknown): string {
	if (typeof v !== 'string') return '';
	return v.replace(/[\r\n]+/g, ' ').trim();
}

function sanitizeLocalPart(v: string) {
	return v
		.trim()
		.toLowerCase()
		.replace(/[^a-z0-9.!#$%&'*+/=?^_`{|}~-]+/g, '-')
		.replace(/^-+|-+$/g, '');
}

function yyyyMmDdUtc(d = new Date()) {
	return d.toISOString().slice(0, 10);
}

function toHexLower(bytes: ArrayBuffer): string {
	const u8 = new Uint8Array(bytes);
	let out = '';
	for (let i = 0; i < u8.length; i++) out += u8[i].toString(16).padStart(2, '0');
	return out;
}

function toBase64(bytes: ArrayBuffer): string {
	let bin = '';
	const u8 = new Uint8Array(bytes);
	for (let i = 0; i < u8.length; i++) bin += String.fromCharCode(u8[i]);
	return btoa(bin);
}

async function sha256HexUtf8(input: string): Promise<string> {
	const data = new TextEncoder().encode(input);
	const hash = await crypto.subtle.digest('SHA-256', data);
	return toHexLower(hash);
}

async function sha256HexBytes(input: ArrayBuffer): Promise<string> {
	const hash = await crypto.subtle.digest('SHA-256', input);
	return toHexLower(hash);
}

async function hmacSha256Base64(secret: string, message: string): Promise<string> {
	const keyBytes = new TextEncoder().encode(secret);
	const key = await crypto.subtle.importKey('raw', keyBytes, { name: 'HMAC', hash: 'SHA-256' }, false, ['sign']);
	const sig = await crypto.subtle.sign('HMAC', key, new TextEncoder().encode(message));
	return toBase64(sig);
}

function findHeaderSeparator(raw: Uint8Array): { index: number; len: number } | null {
	// Prefer CRLFCRLF, fallback to LFLF.
	for (let i = 0; i + 3 < raw.length; i++) {
		if (raw[i] === 13 && raw[i + 1] === 10 && raw[i + 2] === 13 && raw[i + 3] === 10) {
			return { index: i, len: 4 };
		}
	}
	for (let i = 0; i + 1 < raw.length; i++) {
		if (raw[i] === 10 && raw[i + 1] === 10) {
			return { index: i, len: 2 };
		}
	}
	return null;
}

function extractRfc822Header(raw: Uint8Array, headerName: string): string | null {
	const sep = findHeaderSeparator(raw);
	if (!sep) return null;

	const headerBytes = raw.slice(0, sep.index);

	// RFC822 headers are byte-ish; ISO-8859-1 preserves 0–255.
	const dec = new TextDecoder('iso-8859-1');
	const headerText = dec.decode(headerBytes);

	// Unfold continuation lines.
	const unfolded = headerText.replace(/\r?\n[ \t]+/g, ' ');
	const lines = unfolded.split(/\r?\n/);

	const target = headerName.toLowerCase() + ':';
	for (const line of lines) {
		if (line.toLowerCase().startsWith(target)) return line.slice(target.length).trim();
	}
	return null;
}

function rewriteRfc822Headers(
	raw: Uint8Array,
	rewrite: {
		from: string;
		to?: string;
		replyTo?: string;
		addHeaders?: Record<string, string>;
	}
): Uint8Array {
	const sep = findHeaderSeparator(raw);
	if (!sep) return raw; // malformed; don't touch

	const headerBytes = raw.slice(0, sep.index);
	const bodyBytes = raw.slice(sep.index + sep.len); // body WITHOUT the original separator

	const dec = new TextDecoder('iso-8859-1');
	const enc = new TextEncoder();

	const headerText = dec.decode(headerBytes);
	const lines = headerText.split(/\r?\n/);

	const outLines: string[] = [];
	let i = 0;

	function isContinuation(line: string) {
		return line.startsWith(' ') || line.startsWith('\t');
	}

	while (i < lines.length) {
		const start = lines[i];
		if (!start) {
			i++;
			continue;
		}

		const block: string[] = [start];
		i++;
		while (i < lines.length && isContinuation(lines[i])) {
			block.push(lines[i]);
			i++;
		}

		const fieldName = start.split(':')[0]?.trim().toLowerCase();

		if (fieldName === 'from') continue;
		if (fieldName === 'to') continue;
		if (fieldName === 'reply-to') continue;
		if (fieldName === 'return-path') continue;

		outLines.push(...block);
	}

	// Inject rewritten headers at top.
	outLines.unshift(`From: ${safeHeader(rewrite.from)}`);
	if (rewrite.to) outLines.unshift(`To: ${safeHeader(rewrite.to)}`);
	if (rewrite.replyTo) outLines.unshift(`Reply-To: ${safeHeader(rewrite.replyTo)}`);

	if (rewrite.addHeaders) {
		for (const [k, v] of Object.entries(rewrite.addHeaders)) {
			const kk = safeHeader(k).replace(/:$/, '');
			const vv = safeHeader(v);
			if (!kk) continue;
			outLines.push(`${kk}: ${vv}`);
		}
	}

	// Always rebuild with CRLF + CRLF separator (canonical for SMTP).
	const rebuiltHeader = outLines.join('\r\n') + '\r\n\r\n';
	const rebuiltHeaderBytes = enc.encode(rebuiltHeader);

	const merged = new Uint8Array(rebuiltHeaderBytes.length + bodyBytes.length);
	merged.set(rebuiltHeaderBytes, 0);
	merged.set(bodyBytes, rebuiltHeaderBytes.length);
	return merged;
}

function asMailgunFrom(forwardFrom: string, fallbackLocal: string, mailgunDomain: string) {
	const parsed = safeHeader(forwardFrom);
	const m = parsed.match(/<([^>]+)>/);
	const addr = m ? m[1] : parsed;
	const at = addr.lastIndexOf('@');
	const local = at > 0 ? addr.slice(0, at) : addr;
	const safeLocal = sanitizeLocalPart(local) || fallbackLocal;
	return `${safeLocal}@${mailgunDomain}`;
}

async function postSignedJson<TResponse = unknown>(env: Env, path: string, payload: any): Promise<TResponse | null> {
	const url = `${env.API_BASE_URL}${path}`;

	const body = JSON.stringify(payload);
	const xDate = yyyyMmDdUtc();

	const bodyHash = await sha256HexUtf8(body);
	const canonical = ['POST', path, xDate, bodyHash].join('\n');
	const xSignature = await hmacSha256Base64(env.EMAIL_HMAC_SECRET, canonical);

	const res = await fetch(url, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
			'x-date': xDate,
			'x-signature': xSignature,
		},
		body,
	});

	if (res.status === 404) return null;
	if (!res.ok) throw new Error(`API error: ${res.status} ${await res.text()}`);

	const text = await res.text();
	if (!text) return null;
	return JSON.parse(text) as TResponse;
}

/**
 * Signed binary POST.
 * Signature matches server canonicalization:
 *   METHOD + "\n" + PATH(without query) + "\n" + x-date + "\n" + sha256Hex(rawBodyBytes)
 */
async function postSignedBinary(
	env: Env,
	opts: {
		path: string; // NO query
		query?: Record<string, string>;
		bodyBytes: Uint8Array;
		contentType: string;
		contentDisposition?: string;
	}
): Promise<void> {
	const qs =
		opts.query && Object.keys(opts.query).length
			? `?${Object.entries(opts.query)
					.map(([k, v]) => `${encodeURIComponent(k)}=${encodeURIComponent(v)}`)
					.join('&')}`
			: '';

	const url = `${env.API_BASE_URL}${opts.path}${qs}`;

	const xDate = yyyyMmDdUtc();
	const bodyHash = await sha256HexBytes(opts.bodyBytes.buffer);
	const canonical = ['POST', opts.path, xDate, bodyHash].join('\n');
	const xSignature = await hmacSha256Base64(env.EMAIL_HMAC_SECRET, canonical);

	const headers: Record<string, string> = {
		'x-date': xDate,
		'x-signature': xSignature,
		'Content-Type': opts.contentType,
		'Content-Length': String(opts.bodyBytes.byteLength),
	};

	if (opts.contentDisposition) headers['Content-Disposition'] = opts.contentDisposition;

	const res = await fetch(url, {
		method: 'POST',
		headers,
		body: opts.bodyBytes,
	});

	if (!res.ok) throw new Error(`Quarantine upload failed: ${res.status} ${await res.text()}`);
}

async function postToApi(env: Env, payload: any): Promise<ApiDecision | null> {
	log(env, 'info', 'api_post_start', {
		source: payload?.source,
		alias: payload?.alias,
		routingKey: payload?.routingKey,
		domain: payload?.domain,
		messageId: payload?.messageId,
	});

	const response = await postSignedJson<ApiDecision>(env, '/v1/email', payload);

	log(env, 'info', 'api_post_done', {
		action: response?.action ?? null,
		deliveryActionId: response?.deliveryActionId ?? null,
		partitionKey: response?.partitionKey ?? null,
		forwardTo: (response as any)?.forwardTo ?? null,
		forwardFrom: (response as any)?.forwardFrom ?? null,
	});

	return response;
}

type CompleteOutcome = 'Dropped' | 'Quarantined' | 'Forwarded' | 'ForwardFailed';

async function postComplete(
	env: Env,
	deliveryActionId: string,
	payload: {
		deliveryActionId: string;
		partitionKey: string;
		completedAtUtc: string;
		outcome: CompleteOutcome;
		reason?: string;

		alias?: string;
		routingKey?: string;
		domain?: string;
		recipient?: string;
		forwardTo?: string;

		provider?: 'mailgun';
		providerMessageId?: string;
		error?: string;

		source?: string;
	}
): Promise<void> {
	log(env, 'info', 'complete_post_start', {
		deliveryActionId,
		partitionKey: payload.partitionKey,
		outcome: payload.outcome,
		provider: payload.provider ?? null,
		providerMessageId: payload.providerMessageId ?? null,
	});

	await postSignedJson(env, `/v1/email/${deliveryActionId}/complete`, payload);

	log(env, 'info', 'complete_post_done', {
		deliveryActionId,
		partitionKey: payload.partitionKey,
		outcome: payload.outcome,
	});
}

function domainOf(addr: string) {
	const at = addr.lastIndexOf('@');
	return at > -1 ? addr.slice(at + 1).toLowerCase() : '';
}

function firstN(s: string, n: number) {
	return s.length <= n ? s : s.slice(0, n);
}

type MailgunSendResult = {
	status: number;
	rawText: string;
	providerMessageId?: string; // Mailgun "id"
	providerResponseMessage?: string; // Mailgun "message"
};

async function sendViaMailgunMime(
	env: Env,
	opts: {
		to: string;
		rawMime: Uint8Array;
		logCtx: Record<string, unknown>;
	}
): Promise<MailgunSendResult> {
	const url = `${mailgunBase(env.MAILGUN_REGION)}/v3/${env.MAILGUN_DOMAIN}/messages.mime`;
	const auth = btoa(`api:${env.MAILGUN_API_KEY}`);

	const form = new FormData();
	form.append('to', safeHeader(opts.to));

	const eml = new File([opts.rawMime], 'message.eml', { type: 'message/rfc822' });
	form.append('message', eml);

	log(env, 'info', 'mailgun_send_attempt', {
		...opts.logCtx,
		mailgunRegion: env.MAILGUN_REGION,
		mailgunDomain: env.MAILGUN_DOMAIN,
	});

	const res = await fetch(url, {
		method: 'POST',
		headers: { Authorization: `Basic ${auth}` },
		body: form,
	});

	const text = await res.text();

	if (!res.ok) {
		log(env, 'error', 'mailgun_send_failed', {
			...opts.logCtx,
			status: res.status,
			body: firstN(text, 2000),
		});
		throw new Error(`Mailgun mime send failed: ${res.status} ${text}`);
	}

	let parsed: any = null;
	try {
		parsed = text ? JSON.parse(text) : null;
	} catch {
		parsed = null;
	}

	const result: MailgunSendResult = {
		status: res.status,
		rawText: text,
		providerMessageId: parsed?.id ?? undefined,
		providerResponseMessage: parsed?.message ?? undefined,
	};

	log(env, 'info', 'mailgun_send_ok', {
		...opts.logCtx,
		status: res.status,
		mailgunId: result.providerMessageId ?? null,
		mailgunMessage: result.providerResponseMessage ?? firstN(text, 200),
	});

	return result;
}

/**
 * NOTE:
 * - Your API currently returns a string "reason" (used previously as a human message).
 * - The quarantine endpoint requires:
 *     reason: QuarantineReason (enum) + message: string
 *   This worker maps:
 *     reason => "Policy" (string) by default, message => decision.reason
 * - If you want exact enum reasons, change the API to return `quarantineReason` as an enum name.
 */
type ApiDecision =
	| { action: 'Drop'; deliveryActionId: string; partitionKey: string; reason?: string }
	| {
			action: 'Quarantine';
			deliveryActionId: string;
			partitionKey: string;
			reason?: string; // human message (kept)
			quarantineReason?: string; // enum name if your API provides it (optional)
	  }
	| { action: 'Forward'; deliveryActionId: string; partitionKey: string; forwardTo: string; forwardFrom?: string };

export default {
	async email(message: any, env: Env, ctx: ExecutionContext) {
		try {
			const rawRecipient = safeHeader(message.to as string);
			const [localPart, domainPart] = rawRecipient.split('@');
			if (!localPart || !domainPart) return;

			const [routingKey] = domainPart.split('.');
			if (!routingKey) return;

			const alias = safeHeader(localPart);
			const fullDomain = safeHeader(domainPart);

			const receivedAtUtc = new Date().toISOString();

			const sender = safeHeader(message.from);
			const senderDomain = domainOf(sender);

			const recipient = rawRecipient;
			const recipientDomain = domainOf(recipient);

			const messageId = safeHeader(message.headers?.get?.('message-id') ?? '');
			const subject = safeHeader(message.headers?.get?.('subject') ?? '');
			const subjectPreview = subject ? firstN(subject, 200) : undefined;
			const subjectHash = subject ? await hmacSha256Base64(env.EMAIL_HMAC_SECRET, subject) : undefined;

			const hasAttachments = (message.attachments?.length ?? 0) > 0;
			const attachmentsCount = message.attachments?.length ?? 0;
			const size = message.rawSize ?? 0;

			log(env, 'debug', 'log_level_effective', { level: parseLogLevel(env.LOG_LEVEL) });

			const payload = {
				receivedAtUtc,
				messageId: messageId || undefined,

				alias,
				routingKey,
				domain: fullDomain,

				sender,
				senderDomain,
				recipient,
				recipientDomain,

				subject: subjectPreview,
				subjectHash,

				hasAttachments,
				attachmentsCount,
				size,

				source: 'cloudflare-email-routing',
			};

			let decision: ApiDecision | null;
			try {
				decision = await postToApi(env, payload);
			} catch (e: any) {
				log(env, 'error', 'api_post_error', { error: safeHeader(e?.message ?? String(e)) });
				return;
			}

			if (!decision) return;

			const baseCtx = {
				deliveryActionId: decision.deliveryActionId,
				partitionKey: decision.partitionKey,
				alias,
				routingKey,
				domain: fullDomain,
				recipient: rawRecipient,
				originalFrom: sender,
				messageId: messageId || null,
			};

			// Drop: unchanged (no upload)
			if (decision.action === 'Drop') {
				log(env, 'info', 'decision_terminal', { ...baseCtx, action: decision.action, reason: decision.reason ?? null });

				ctx.waitUntil(
					postComplete(env, decision.deliveryActionId, {
						deliveryActionId: decision.deliveryActionId,
						partitionKey: decision.partitionKey,
						completedAtUtc: new Date().toISOString(),
						outcome: 'Dropped',
						reason: decision.reason,

						alias,
						routingKey,
						domain: fullDomain,
						recipient: rawRecipient,

						source: 'cloudflare-email-routing',
					}).catch((err) => log(env, 'error', 'complete_post_error', { ...baseCtx, error: safeHeader(err?.message ?? String(err)) }))
				);
				return;
			}

			// Quarantine: NEW - upload .eml to quarantine endpoint
			if (decision.action === 'Quarantine') {
				log(env, 'info', 'decision_terminal', { ...baseCtx, action: decision.action, reason: decision.reason ?? null });

				ctx.waitUntil(
					(async () => {
						try {
							if (!message.raw) throw new Error('Missing raw MIME (message.raw)');

							const rawBytes = new Uint8Array(await new Response(message.raw).arrayBuffer());

							// Best-effort filename (safe enough; server parses Content-Disposition if present)
							const fileName = 'message.eml';

							// Map to your API query contract:
							//   reason (enum) + message (string)
							// If your API returns `quarantineReason` as an enum name, use it; else default.
							const quarantineReason = safeHeader((decision as any).quarantineReason ?? 'Policy');
							const quarantineMessage = safeHeader(decision.reason ?? 'Quarantined by policy');

							await postSignedBinary(env, {
								path: `/v1/email/${decision.deliveryActionId}/quarantine`,
								query: {
									partitionKey: decision.partitionKey,
									reason: quarantineReason,
									message: quarantineMessage,
								},
								bodyBytes: rawBytes,
								contentType: 'message/rfc822',
								contentDisposition: `attachment; filename="${fileName}"`,
							});

							await postComplete(env, decision.deliveryActionId, {
								deliveryActionId: decision.deliveryActionId,
								partitionKey: decision.partitionKey,
								completedAtUtc: new Date().toISOString(),
								outcome: 'Quarantined',
								reason: decision.reason,

								alias,
								routingKey,
								domain: fullDomain,
								recipient: rawRecipient,

								source: 'cloudflare-email-routing',
							});
						} catch (err: any) {
							log(env, 'error', 'quarantine_upload_error', { ...baseCtx, error: safeHeader(err?.message ?? String(err)) });

							// Still mark completed (failed path) so the system can surface it
							try {
								await postComplete(env, decision.deliveryActionId, {
									deliveryActionId: decision.deliveryActionId,
									partitionKey: decision.partitionKey,
									completedAtUtc: new Date().toISOString(),
									outcome: 'Quarantined',
									reason: decision.reason,

									alias,
									routingKey,
									domain: fullDomain,
									recipient: rawRecipient,

									error: safeHeader(err?.message ?? String(err)),
									source: 'cloudflare-email-routing',
								});
							} catch (e: any) {
								log(env, 'error', 'complete_post_error_after_quarantine_upload_error', {
									...baseCtx,
									error: safeHeader(e?.message ?? String(e)),
								});
							}
						}
					})()
				);

				return;
			}

			// Forward: unchanged
			const forwardTo = safeHeader((decision as any).forwardTo);
			if (!forwardTo) return;
			if (!message.raw) return;

			const rawBytes = new Uint8Array(await new Response(message.raw).arrayBuffer());

			const fallbackLocal = `${sanitizeLocalPart(alias)}+${sanitizeLocalPart(routingKey)}`;
			const desiredFrom = (decision as any).forwardFrom
				? asMailgunFrom((decision as any).forwardFrom, fallbackLocal, env.MAILGUN_DOMAIN)
				: `${fallbackLocal}@${env.MAILGUN_DOMAIN}`;

			log(env, 'info', 'forward_prepare', {
				...baseCtx,
				forwardTo,
				decisionForwardFrom: (decision as any).forwardFrom ?? null,
				fallbackLocal,
				desiredFrom,
			});

			const rewritten = rewriteRfc822Headers(rawBytes, {
				from: desiredFrom,
				to: forwardTo,
				replyTo: sender,
				addHeaders: {
					'X-Nullbox-Alias': alias,
					'X-Nullbox-RoutingKey': routingKey,
					'X-Nullbox-Original-To': rawRecipient,
					'X-Nullbox-Original-From': sender,
				},
			});

			const mimeFrom = extractRfc822Header(rewritten, 'From');
			const mimeTo = extractRfc822Header(rewritten, 'To');
			const mimeReplyTo = extractRfc822Header(rewritten, 'Reply-To');

			log(env, 'info', 'forward_mime_headers', {
				...baseCtx,
				forwardTo,
				desiredFrom,
				mimeFrom: mimeFrom ?? null,
				mimeTo: mimeTo ?? null,
				mimeReplyTo: mimeReplyTo ?? null,
				fromMatchesDesired: (mimeFrom ?? '') === desiredFrom,
			});

			ctx.waitUntil(
				(async () => {
					try {
						const mg = await sendViaMailgunMime(env, {
							to: forwardTo,
							rawMime: rewritten,
							logCtx: {
								...baseCtx,
								forwardTo,
								desiredFrom,
								mimeFrom: mimeFrom ?? null,
								fromMatchesDesired: (mimeFrom ?? '') === desiredFrom,
							},
						});

						await postComplete(env, (decision as any).deliveryActionId, {
							deliveryActionId: (decision as any).deliveryActionId,
							partitionKey: (decision as any).partitionKey,
							completedAtUtc: new Date().toISOString(),
							outcome: 'Forwarded',

							alias,
							routingKey,
							domain: fullDomain,
							recipient: rawRecipient,
							forwardTo,

							provider: 'mailgun',
							providerMessageId: mg.providerMessageId ?? undefined,
							source: 'cloudflare-email-routing',
						});
					} catch (err: any) {
						log(env, 'error', 'forward_pipeline_error', { ...baseCtx, forwardTo, error: safeHeader(err?.message ?? String(err)) });

						try {
							await postComplete(env, (decision as any).deliveryActionId, {
								deliveryActionId: (decision as any).deliveryActionId,
								partitionKey: (decision as any).partitionKey,
								completedAtUtc: new Date().toISOString(),
								outcome: 'ForwardFailed',

								alias,
								routingKey,
								domain: fullDomain,
								recipient: rawRecipient,
								forwardTo,

								provider: 'mailgun',
								error: safeHeader(err?.message ?? String(err)),
								source: 'cloudflare-email-routing',
							});
						} catch (e: any) {
							log(env, 'error', 'complete_post_error_after_failure', {
								...baseCtx,
								forwardTo,
								error: safeHeader(e?.message ?? String(e)),
							});
						}
					}
				})()
			);
		} catch (err: any) {
			log(env, 'error', 'worker_error', { error: safeHeader(err?.message ?? String(err)) });
			return;
		}
	},
};
