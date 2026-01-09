import { decode } from "@tsndr/cloudflare-worker-jwt";

type JwtPayload = { exp?: number }
type SessionTokens = {
  id_token?: string
  refresh_token?: string
  access_token?: string
  token_type?: string
  expires_in?: number
  scope?: string
}

function getExpirySkewMs() {
  // Small safety window to account for clock skew/network latency (default 5 minutes)
  const DEFAULT_SKEW = 5 * 60 * 1000
  const fromEnv = process.env.NUXT_TOKEN_EXPIRY_SKEW_MS
  const parsed = fromEnv ? Number(fromEnv) : NaN
  return Number.isFinite(parsed) && parsed >= 0 ? parsed : DEFAULT_SKEW
}

function isTokenExpired(token: string): boolean {
  // JWT exp is a Unix timestamp (seconds since epoch, UTC). Timezone does not affect this.
  const decoded = decode(token)
  const expiresAtMs = (decoded?.exp ?? 0) * 1000
  if (!expiresAtMs) return true
  const skew = getExpirySkewMs()
  return Date.now() >= (expiresAtMs - skew)
}

async function refreshTokensWithEntraExternal(refreshToken: string) {
  const config = useRuntimeConfig()
  const entra = config.oauth?.entraexternal as
    | { clientId?: string; tenant?: string; tenantId?: string; tokenURL?: string; scope?: string[] }
    | undefined

  // Fallback to env vars if runtimeConfig is not populated for some reason
  const clientId = entra?.clientId || process.env.NUXT_OAUTH_ENTRAEXTERNAL_CLIENT_ID
  const tenant = entra?.tenant || process.env.NUXT_OAUTH_ENTRAEXTERNAL_TENANT
  const tenantId = entra?.tenantId || process.env.NUXT_OAUTH_ENTRAEXTERNAL_TENANT_ID
  const tokenURL =
    entra?.tokenURL || (tenant && tenantId ? `https://${tenant}.ciamlogin.com/${tenantId}/oauth2/v2.0/token` : undefined)
  const scope = (entra?.scope && entra.scope.length > 0 ? entra.scope : ['openid', 'profile', 'email', 'offline_access']).join(' ')

  if (!clientId || !tokenURL) {
    throw new Error('Entra External OAuth config is missing clientId or tokenURL/tenant settings')
  }

  // Public client with PKCE typically does NOT use client_secret for refresh
  return $fetch<any>(tokenURL, {
    method: 'POST',
    headers: { 'content-type': 'application/x-www-form-urlencoded' },
    body: new URLSearchParams({
      grant_type: 'refresh_token',
      client_id: clientId,
      refresh_token: refreshToken,
      scope,
    }).toString(),
  })
}

export default defineEventHandler(async (event) => {
  const session: UserSession = await getUserSession(event)
  if (!session?.tokens) {
    return
  }

  const tokens = session.tokens as unknown as SessionTokens
  const tokenToCheck = tokens.id_token
  if (!tokenToCheck) return

  const isIdTokenExpired = isTokenExpired(tokenToCheck)
  if (!isIdTokenExpired) {
    // Still valid (considering small skew); no refresh needed yet
    return
  }

  try {
    if (!tokens.refresh_token) throw new Error('Missing refresh_token in session')
    const newTokens = (await refreshTokensWithEntraExternal(tokens.refresh_token)) as Partial<SessionTokens>

    // Update session with fresh tokens (limit to what we store)
    await setUserSession(event, {
      tokens: {
        id_token: newTokens.id_token,
        refresh_token: newTokens.refresh_token || tokens.refresh_token,
      },
    })
  } catch (error) {
    console.error('Failed to refresh tokens:', error)
    await clearUserSession(event)
  }
})