<script setup lang="ts">
const { t } = useI18n({ useScope: "local" });

// number of quote options in i18n
const QUOTE_COUNT = 14;

const colorMode = useColorMode();

const quotes = computed(() => {
  const options = [];
  for (let i = 0; i < QUOTE_COUNT; i++) {
    options.push(t(`quote.options.${i}`));
  }
  return options;
});

const particleColorDark = [225, 225, 225];
const particleColorLight = [0, 0, 0];

const particleRGBColor = computed(() =>
  colorMode.value === "dark" ? particleColorDark : particleColorLight
);

// --- header/menu state (inside blackhole) ---
type NavItem = { name: string; href: string };

const navigation: NavItem[] = [
  { name: "Features", href: "#features-01" },
  { name: "FAQ", href: "#faq" },
];

const mobileOpen = ref(false);
</script>

<template>
  <div id="hero" class="relative h-screen w-full overflow-hidden">
    <BlackHoleBackground
      class="absolute inset-0 rounded-xl"
      :particleRGBColor="particleRGBColor"
    >
      <!-- Header INSIDE the blackhole -->
      <site-header />

      <!-- Hero content (pad top so it clears header) -->
      <div
        class="relative z-20 mx-auto flex h-full max-w-2xl flex-col items-center justify-center px-6 pt-24 text-center lg:px-8"
      >
        <div id="header-background" />
        <img
          src="/assets/logo.light.svg"
          alt="Nullbox Logo"
          class="block h-48 w-auto dark:hidden"
        />
        <img
          src="/assets/logo.dark.svg"
          alt="Nullbox Logo"
          class="hidden h-48 w-auto dark:block"
        />

        <p class="mt-4 text-2xl leading-7">
          {{ t("description") }}
        </p>

        <span class="mt-10 text-xl">
          <container-text-flip :words="quotes" />
        </span>

        <div class="mt-10 flex flex-col items-center gap-3 sm:flex-row">
          <Button as-child>
            <NuxtLink to="/get-started" external>
              {{ t("cta.primary") }}
            </NuxtLink>
          </Button>
          <Button variant="secondary" as-child>
            <a href="#features-01">
              {{ t("cta.secondary") }}
            </a>
          </Button>
        </div>
      </div>
    </BlackHoleBackground>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  title: "Nullbox"
  brand: "Nullbox"
  description: "Private inboxes with aliases. Keep your real address out of forms, lists, and leaks."
  cta:
    primary: Get started
    secondary: Learn more
  quote:
    options:
      - Privacy isn’t a feature. It’s the foundation.
      - Your information belongs to you. We keep it that way.
      - Start privately. Stay private.
      - Strong privacy, minimal friction.
      - Stay reachable without being exposed.
      - Give out an email, not your identity.
      - Your inbox should answer to you, not advertisers.
      - Keep your real address off the front lines.
      - Control what reaches you, and what doesn’t.
      - Less tracking. Same email.
      - Block the data grab, keep the messages.
      - A safer inbox starts with a little distance.
      - Good tools protect you, even when you’re not looking.
      - Privacy is freedom in the digital world.
</i18n>
