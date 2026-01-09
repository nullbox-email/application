import { decode } from "@tsndr/cloudflare-worker-jwt";

export default defineNuxtRouteMiddleware(async () => {
  const nuxtApp = useNuxtApp();
  if (
    import.meta.client &&
    nuxtApp.isHydrating &&
    nuxtApp.payload.serverRendered
  )
    return;

  const {
    session,
    clear: clearSession,
    fetch: fetchSession,
  } = useUserSession();
  const tokens = (session.value as any)?.tokens as any;
  if (!tokens?.id_token) return;

  const idToken = tokens.id_token as string;
  const refreshToken = tokens.refresh_token as string | undefined;
  let exp: number | undefined;
  try {
    const decoded: any = decode(idToken);
    exp = decoded?.payload?.exp;
  } catch {
    exp = undefined;
  }
  const nowSec = Math.floor(Date.now() / 1000);
  // Refresh if the token has no exp or will expire within 60 seconds
  const needsRefresh = exp == null || exp <= nowSec + 60;
  if (!needsRefresh) return;
  if (!refreshToken) {
    await clearSession();
    return;
  }
  try {
    await useRequestFetch()("/api/jwt/refresh", { method: "POST" });
    await fetchSession();
  } catch (err) {
    console.warn(
      "[Middleware] Automatic refresh failed; redirecting to login",
      err
    );
    const provider = (session.value as any)?.provider as string | undefined;
    await clearSession();
    return navigateTo(provider ? `/auth/${provider}` : "/auth");
  }
});
