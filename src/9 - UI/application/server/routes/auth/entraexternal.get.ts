import { IdToken } from "~~/server/types/security/IdToken";

export function parseJwt(token: string) {
  var base64Url = token.split(".")[1];
  var base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
  var jsonPayload = decodeURIComponent(
    atob(base64)
      .split("")
      .map(function (c) {
        return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2);
      })
      .join("")
  );

  return JSON.parse(jsonPayload);
}

export default defineOAuthEntraExternalEventHandler({
  config: {
    scope: ["openid profile email offline_access"],
  },
  async onSuccess(event, { user, tokens }) {
    try {
      const config = useRuntimeConfig();

      // Exchange the external token for your own JWT
      const { idToken } = await $fetch<IdToken>(
        `${config.public.apiUrl}/v1/tokens/exchange`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${tokens.id_token}`,
          },
        }
      );

      const identity = parseJwt(idToken);

      const name = identity[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
          ] ?? undefined;

      const identityUserProfile = {
        id: identity.sub,
        name: name,          
        initials: name
          .split(" ")
          .map((n) => n[0])
          .join("")
          .toUpperCase(),
        email:
          identity[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
          ] ?? undefined,
        flags: {
          restricted_access:
            identity.permissions?.includes("restricted_access") ?? false,
          profile_incomplete:
            identity["https://nullbox.email/claims/user/status"] === "new",
        },
        status: identity["https://nullbox.email/claims/user/status"],
      };

      await setUserSession(event, {
        user: identityUserProfile,
        loggedInAt: Date.now(),
        tokens: {
          idToken,
        },
      });

      // Set the JWT in an HTTP-only cookie
      setCookie(event, "nullbox.auth", JSON.stringify({ idToken }), {
        httpOnly: false,
        secure: process.env.NODE_ENV === "production",
        sameSite: "lax",
        path: "/",
      });

      // Redirect based on user existence
      if (identityUserProfile.flags.profile_incomplete) {
        return sendRedirect(event, "/on-boarding");
      }

      return sendRedirect(event, "/");
    } catch (error) {
      throw createError({
        statusCode: 500,
        statusMessage: error.message,
        data: error,
        fatal: true,
      });
    }
  },
});
