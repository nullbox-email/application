// server/middleware/forwarded-host.ts
export default defineEventHandler((event) => {
  const headers = event.node.req.headers

  const forwardedHost =
    (headers['x-forwarded-host'] as string | undefined) ??
    (headers['x-original-host'] as string | undefined)

  if (forwardedHost) {
    // make Nuxt think the request was for the original host
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    // @ts-expect-error: Node's IncomingHttpHeaders is not strictly writable
    headers.host = forwardedHost
  }

  // If you care about scheme for absolute URLs, redirects, etc.
  if (headers['x-forwarded-proto']) {
    // Nitro/H3 helpers already look at this when computing request URL
    // so you normally don't need extra handling here.
  }
})
