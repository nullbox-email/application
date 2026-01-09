<script setup lang="ts">
const { t } = useI18n({ useScope: "local" });

defineProps({learnMoreUrl: {type: String, required: true}})
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
        <span class="text-2xl">{{ t("hero.title.line2") }}</span>
      </h2>

      <div class="mt-10 flex items-center gap-x-6 lg:mt-0 lg:shrink-0">
        <Button as-child>
          <nuxt-link to="/get-started" external>
            {{ t("cta.primary") }}
          </nuxt-link>
        </Button>

        <Button variant="secondary" as-child>
          <nuxt-link :to="learnMoreUrl">
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
      line1: Stop spam at the source
      line2: Use one alias per site and rotate anytime.
  cta:
    primary: Create your first alias
    secondary: Learn more
</i18n>
