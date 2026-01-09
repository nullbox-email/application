// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: "2025-07-15",
  devtools: { enabled: true },

  runtimeConfig: {
    public: {
      apiUrl: "http://localhost:6500",
      entraExternalTenant: "",
      entraExternalTenantId: "",
      version: "dev",
    },
    turnstile: {
      // This can be overridden at runtime via the NUXT_TURNSTILE_SECRET_KEY
      // environment variable.
      secretKey: "",
    },
  },

  modules: [
    "@nuxt/icon",
    "@nuxt/image",
    "@nuxt/fonts",
    "@nuxtjs/color-mode",
    "@nuxtjs/i18n",
    "@nuxtjs/tailwindcss",
    "nuxt-auth-utils",
    "nuxt-security",
    "shadcn-nuxt",
    "@nuxtjs/turnstile",
  ],

  app: {
    head: {
      title: "Nullbox",
      titleTemplate: "%s - Nullbox",
      charset: "utf-8",
      viewport: "width=device-width, initial-scale=1",
      link: [{ rel: "icon", type: "images/svg", href: "/favicon.svg" }],
    },
  },

  // https://nuxt.com/modules/color-mode
  colorMode: {
    preference: "system",
    fallback: "light",
    classSuffix: "",
  },

  // https://nuxt.com/modules/fonts
  fonts: {
    families: [
      // only resolve this font with the `google` provider
      { name: "Inter", provider: "google", global: true },
    ],
    provider: "google",
  },

  // https://nuxt.com/modules/i18n
  i18n: {
    locales: ["en"],
    defaultLocale: "en",
  },

  // https://nuxt.com/modules/icon
  icon: {
    customCollections: [
      {
        prefix: "nullbox",
        dir: "./assets/icons/",
      },
    ],
  },

  // https://nuxt.com/modules/image
  image: {},

  // https://nuxt.com/modules/security
  security: {
    corsHandler: {
      // Your public-facing origin
      origin: "https://app.nullbox.email",
    },
  },

  // https://nuxt.com/modules/shadcn
  shadcn: {
    /**
     * Prefix for all the imported component.
     * @default "Ui"
     */
    prefix: "",
    /**
     * Directory that the component lives in.
     * Will respect the Nuxt aliases.
     * @link https://nuxt.com/docs/api/nuxt-config#alias
     * @default "@/components/ui"
     */
    componentDir: "@/components/ui",
  },

  // https://nuxt.com/modules/turnstile
  turnstile: {
    siteKey: "<your-site-key>",
  },
});
