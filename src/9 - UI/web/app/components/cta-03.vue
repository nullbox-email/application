<script setup lang="ts">
const { t } = useI18n({ useScope: "local" });
</script>

<template>
  <div class="bg-primary/35">
    <div
      class="mx-auto max-w-7xl px-6 py-24 sm:py-32 lg:flex lg:items-center lg:justify-between lg:px-8"
    >
      <h2
        class="max-w-2xl text-4xl font-semibold tracking-tight text-foreground sm:text-5xl"
      >
        {{ t("hero.title.line1") }}
        <br />
        {{ t("hero.title.line2") }}
      </h2>

      <div class="mt-10 flex items-center gap-x-6 lg:mt-0 lg:shrink-0">
        <Button as-child>
          <nuxt-link to="/get-started" external>
            {{ t("cta.primary") }}
          </nuxt-link>
        </Button>

        <Button variant="secondary" as-child>
          <nuxt-link to="#faq">
            {{ t("cta.secondary") }}
          </nuxt-link>
        </Button>
      </div>
    </div>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  hero:
    title:
      line1: Ready to dive in?
      line2: Start your free trial today.
  cta:
    primary: Get started
    secondary: Learn more
</i18n>
