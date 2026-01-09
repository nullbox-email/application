<script setup lang="ts">
const { t } = useI18n({ useScope: "local" });
</script>

<template>
  <div class="bg-primary/35">
    <div class="px-6 py-24 sm:py-32 lg:px-8">
      <div class="mx-auto max-w-2xl text-center">
        <h2
          class="text-4xl font-semibold tracking-tight text-balance text-foreground sm:text-5xl"
        >
          {{ t("cta.title") }}
        </h2>

        <p class="mx-auto mt-6 max-w-xl text-lg/8 text-pretty text-foreground">
          {{ t("cta.description") }}
        </p>

        <div class="mt-10 flex items-center justify-center gap-x-6">
          <Button as-child>
            <nuxt-link to="/get-started" external>
              {{ t("cta.primary") }}
            </nuxt-link>
          </Button>

          <Button variant="secondary" as-child>
            <nuxt-link to="https://docs.nullbox.email" external>
              {{ t("cta.secondary") }}
            </nuxt-link>
          </Button>
        </div>
      </div>
    </div>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  cta:
    title: Stop spam at the source.
    description: Create one alias per site, keep your real address private, and shut down spam instantly—without changing how you use email.
    primary: Get started
    secondary: Learn more
</i18n>
