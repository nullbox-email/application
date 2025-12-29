export default defineNuxtRouteMiddleware(async (to, from) => {
  const { loggedIn, user } = useUserSession();

  // Always skip checking if auth route
  if (to.path.startsWith(`/auth/`)) return;

  // Skip if route explicitly sets auth to false
  if (to.meta.auth === false) return;

  if (!loggedIn.value) {
    return navigateTo(`/auth/entraexternal`, { external: true });
  }

  // Always skip checking if onboarding route
  if (to.path.startsWith(`/on-boarding`)) return;

  if (user.value) {
    if (user.value.flags.profile_incomplete) {
      return navigateTo(`/on-boarding`);
    }
  }
});
