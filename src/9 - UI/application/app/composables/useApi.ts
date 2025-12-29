// ~/composables/useApi.ts
import type { UseFetchOptions } from "#app";
import { createError } from "h3";
import { endpoints } from "~/constants/endpoints";
import { AUDIENCES } from "~/constants/audiences";
import type { AccessToken } from "~/types/security/access_token";
import type { IdentityToken } from "~/types/security/identity_token";
import type { Endpoint, PathParams, QueryParams } from "~/lib/endpoint";

type HeadersInitLoose = HeadersInit | undefined;

interface ApiOptions<T> extends UseFetchOptions<T> {
  params?: PathParams<string>;
  query?: QueryParams;
  audience?: string;
  scopes?: string[];
}

type ProblemDetails = {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  [key: string]: any;
};

const SKEW_MS = 15_000;
const DEFAULT_TTL_MS = 55 * 60_000;

const memCache = new Map<string, { token: string; exp: number }>();
const inflight = new Map<string, Promise<string>>();

function normalizeScopes(scopes?: string[]) {
  return [...new Set(scopes || [])].sort();
}

function parseJwtPayload(jwt: string): any | null {
  try {
    const [, payload] = jwt.split(".");
    return JSON.parse(atob(payload.replace(/-/g, "+").replace(/_/g, "/")));
  } catch {
    return null;
  }
}

function readSS(key: string): string | null {
  return import.meta.client ? sessionStorage.getItem(key) : null;
}
function writeSS(key: string, val: string) {
  if (import.meta.client) sessionStorage.setItem(key, val);
}

function expiryFromTokenOrResponse(token: string, expiresIn?: number): number {
  const payload = parseJwtPayload(token);
  if (payload?.exp) return payload.exp * 1000 - SKEW_MS;
  if (typeof expiresIn === "number")
    return Date.now() + Math.max(0, expiresIn * 1000 - SKEW_MS);
  return Date.now() + DEFAULT_TTL_MS;
}

function tokenCacheKey(audience: string, scopes: string[], userSub: string) {
  return `access:${audience}:${scopes.join(" ")}:${userSub || "u"}`;
}

async function fetchAccessToken(
  apiUrl: string,
  identityToken: string,
  audience: string,
  scopes: string[]
) {
  const res = await $fetch<AccessToken>(
    `${apiUrl}${endpoints.AUTH.REQUEST_ACCESS.path}`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${identityToken}`,
      },
      body: { audience, scopes },
    }
  );

  const exp = expiryFromTokenOrResponse(
    // @ts-expect-error allow broader shapes
    (res as any).accessToken,
    // @ts-expect-error allow broader shapes
    (res as any).expiresIn
  );

  return { token: (res as any).accessToken as string, exp };
}

async function getCachedAccessToken(
  apiUrl: string,
  audience: string,
  scopes: string[],
  authIdentityToken: IdentityToken
): Promise<string> {
  const scopesNorm = normalizeScopes(scopes);
  const idPayload = parseJwtPayload(authIdentityToken.idToken);
  const userSub = idPayload?.sub || "user";
  const key = tokenCacheKey(audience, scopesNorm, userSub);

  const hit = memCache.get(key);
  const now = Date.now();
  if (hit && now < hit.exp) return hit.token;

  const ssToken = readSS(`${key}:v`);
  const ssExpRaw = readSS(`${key}:e`);
  if (ssToken && ssExpRaw && now < parseInt(ssExpRaw, 10)) {
    memCache.set(key, { token: ssToken, exp: parseInt(ssExpRaw, 10) });
    return ssToken;
  }

  const existing = inflight.get(key);
  if (existing) return existing;

  const p = (async () => {
    const { token, exp } = await fetchAccessToken(
      apiUrl,
      authIdentityToken.idToken,
      audience,
      scopesNorm
    );
    memCache.set(key, { token, exp });
    writeSS(`${key}:v`, token);
    writeSS(`${key}:e`, String(exp));
    inflight.delete(key);
    return token;
  })().catch((e) => {
    inflight.delete(key);
    throw e;
  });

  inflight.set(key, p);
  return p;
}

function buildUrl<P extends string>(
  endpoint: Endpoint<P> | P,
  params?: PathParams<P>,
  query?: QueryParams
) {
  if (typeof endpoint === "string") {
    let url = endpoint as string;
    if (params) {
      for (const [k, v] of Object.entries(params)) {
        url = url.replace(`:${k}`, encodeURIComponent(String(v)));
      }
    }
    if (query) {
      const qs = Object.entries(query)
        .filter(([, v]) => v !== null && v !== undefined)
        .map(
          ([k, v]) =>
            `${encodeURIComponent(k)}=${encodeURIComponent(String(v))}`
        )
        .join("&");
      if (qs) url += `?${qs}`;
    }
    return url;
  }
  return endpoint.build(params, query);
}

export function useApi<T>(
  endpoint: Endpoint<string> | string,
  options?: ApiOptions<T>
) {
  const config = useRuntimeConfig();
  const {
    params,
    query,
    audience = AUDIENCES.NULLBOX,
    scopes = [],
    ...fetchOptions
  } = options || {};

  const url = buildUrl(endpoint, params, query);

  const userOnRequest = fetchOptions.onRequest;
  const userOnResponse = fetchOptions.onResponse;
  const userOnResponseError = fetchOptions.onResponseError;

  return useFetch<T>(url, {
    baseURL: config.public.apiUrl,
    ...fetchOptions,

    // Always attach Authorization; then let user hook run.
    async onRequest(ctx) {
      const { session } = useUserSession();
      const identity: IdentityToken | null = session.value?.tokens || null;

      if (!identity) {
        throw createError({
          statusCode: 401,
          statusMessage: "User is not authenticated!",
        });
      }

      const accessToken = import.meta.server
        ? (
            await fetchAccessToken(
              config.public.apiUrl,
              identity.idToken,
              audience,
              normalizeScopes(scopes)
            )
          ).token
        : await getCachedAccessToken(
            config.public.apiUrl,
            audience,
            scopes,
            identity
          );

      const requestOptions = ctx.options;
      requestOptions.headers = new Headers(
        requestOptions.headers as HeadersInitLoose
      );
      (requestOptions.headers as Headers).set(
        "Authorization",
        `Bearer ${accessToken}`
      );

      if (typeof userOnRequest === "function") {
        await userOnRequest(ctx as any);
      }
    },

    async onResponse(ctx) {
      if (typeof userOnResponse === "function") {
        await userOnResponse(ctx as any);
      }
    },

    async onResponseError(ctx) {
      // If caller provided a handler, they fully override default error handling.
      if (typeof userOnResponseError === "function") {
        await userOnResponseError(ctx as any);
        return;
      }

      const { response } = ctx;
      const raw = response._data as ProblemDetails | any;
      const status = response.status;

      let message = response.statusText || "Request failed";

      if (raw && typeof raw === "object") {
        if (typeof raw.detail === "string") {
          message = raw.detail;
        } else if (typeof raw.title === "string") {
          message = raw.title;
        }
      }

      // Simple emphasis on 401 / 404 / 500+ via message + fatal flag.
      if (status === 401 && !message) {
        message = "Unauthorized";
      } else if (status === 404 && !message) {
        message = "Not found";
      } else if (status >= 500 && !message) {
        message = "Server error";
      }

      throw createError({
        statusCode: status,
        statusMessage: message,
        data: raw ?? null,
        fatal: status >= 500,
      });
    },
  });
}
