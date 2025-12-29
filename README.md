# Nullbox application

This repo contains multiple services/apps (Aspire AppHost, .NET APIs, Nuxt apps, and a Cloudflare Email Worker).

## Secrets & configuration

### 1) Rotate leaked secrets
If this repo has ever been pushed publicly, assume the following values are compromised and rotate them anywhere they were used:
- `NUXT_SESSION_PASSWORD` (Nuxt session encryption)
- `EmailIngress:HmacSecret` / `EMAIL_HMAC_SECRET` (email ingress request signing)
- `Security.Bearer:IssuerSigningKey` (JWT signing secret)
- `MAILGUN_API_KEY` (Mailgun API key)

### 2) Local development

**Nuxt (`src/9 - UI/application` and `src/9 - UI/web`)**
- Copy `.env.development` to `.env.development.local` and set:
  - `NUXT_SESSION_PASSWORD` (required; 32+ random characters)

**.NET APIs**
Use `.NET user-secrets` (recommended) or environment variables.

- Security API (`src/1 - Security/Nullbox.Security/Nullbox.Security.Api`)
  - `Security.Bearer:IssuerSigningKey` (required; JWT signing secret)
  - `Security.Bearer:HmacKey` (required; key id / rotation identifier)
- Fabric API (`src/2 - Fabric/Nullbox.Fabric/Nullbox.Fabric.Api`)
  - `EmailIngress:HmacSecret` (required; must match the Cloudflare worker `EMAIL_HMAC_SECRET`)

Example (PowerShell):
```
dotnet user-secrets set "Security.Bearer:IssuerSigningKey" "<random-secret>" --project "src/1 - Security/Nullbox.Security/Nullbox.Security.Api/Nullbox.Security.Api.csproj"
dotnet user-secrets set "Security.Bearer:HmacKey" "<kid-guid>" --project "src/1 - Security/Nullbox.Security/Nullbox.Security.Api/Nullbox.Security.Api.csproj"
dotnet user-secrets set "EmailIngress:HmacSecret" "<random-secret>" --project "src/2 - Fabric/Nullbox.Fabric/Nullbox.Fabric.Api/Nullbox.Fabric.Api.csproj"
```

### 3) Cloudflare Email Worker secrets
Worker env vars are defined in `src/workers/blue-fire-0930/src/index.ts`:
- `EMAIL_HMAC_SECRET` (required; must match `EmailIngress:HmacSecret`)
- `MAILGUN_API_KEY` (required)
- `MAILGUN_DOMAIN` (required)
- `MAILGUN_REGION` (required; `us` or `eu`)

Local dev: copy `src/workers/blue-fire-0930/.env.example` to `src/workers/blue-fire-0930/.env`.

Deploy: set secrets with Wrangler (example):
```
cd "src/workers/blue-fire-0930"
wrangler secret put EMAIL_HMAC_SECRET
wrangler secret put MAILGUN_API_KEY
```

### 4) GitHub Actions (production deploy)
Workflow: `.github/workflows/production.yml`

Create a GitHub Environment named `production` and set these **Variables**:
- `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`
- `AZURE_ENV_NAME`, `AZURE_LOCATION`
- `PUBLIC_API_URL`
- `OAUTH_ENTRAEXTERNAL_CLIENT_ID`, `OAUTH_ENTRAEXTERNAL_TENANT`, `OAUTH_ENTRAEXTERNAL_TENANT_ID`
- `TOKEN_EXPIRY_SKEW_MS`

Set these **Secrets** (used as Aspire parameters by `.github/workflows/production.yml`):
- `SECURITY_BEARER_ISSUER_SIGNING_KEY`
- `SECURITY_BEARER_HMAC_KEY`
- `EMAIL_INGRESS_HMAC_SECRET`
- `NUXT_SESSION_PASSWORD`

You can also store sensitive runtime values (JWT signing secret, email ingress HMAC secret, connection strings, etc.) in Azure Key Vault and load them via the app’s `KeyVault:*` configuration.

Key Vault secret naming uses `--` to represent `:` (example: `EmailIngress--HmacSecret`). Note: this repo uses `Security.Bearer:*` keys (dot in the section name), so prefer supplying those via Aspire parameters/env vars unless you’ve standardized your Key Vault naming/mapping.
