<script setup lang="ts">
const { t } = useI18n({ useScope: "local" })

const reasons = [
  {
    name: t("reasons.items.0.name"),
    description: t("reasons.items.0.description"),
    icon: "solar:database-bold-duotone",
  },
  {
    name: t("reasons.items.1.name"),
    description: t("reasons.items.1.description"),
    icon: "solar:shield-warning-bold-duotone",
  },
  {
    name: t("reasons.items.2.name"),
    description: t("reasons.items.2.description"),
    icon: "solar:user-block-rounded-bold-duotone",
  },
  {
    name: t("reasons.items.3.name"),
    description: t("reasons.items.3.description"),
    icon: "solar:eye-scan-bold-duotone",
  },
  {
    name: t("reasons.items.4.name"),
    description: t("reasons.items.4.description"),
    icon: "solar:refresh-bold-duotone",
  },
]
</script>

<template>
  <div id="features-02" class="overflow-hidden bg-background py-24 sm:py-32 lg:py-40">
    <div class="mx-auto max-w-7xl px-6 lg:px-8">
      <div
        class="grid grid-cols-1 gap-x-8 gap-y-16 sm:gap-y-20 lg:grid-cols-2 lg:items-start"
      >
        <div class="lg:pt-4 lg:pr-4">
          <div class="mx-auto max-w-2xl lg:mx-0 lg:max-w-lg">
            <h2 class="text-sm font-semibold text-foreground/80">
              {{ t("reasons.kicker") }}
            </h2>

            <p class="mt-2 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl">
              {{ t("reasons.title") }}
            </p>

            <p class="mt-6 text-lg leading-8 text-muted-foreground">
              {{ t("reasons.subtitle") }}
            </p>

            <dl class="mt-10 max-w-xl space-y-8 text-base leading-7 text-muted-foreground lg:max-w-none">
              <div v-for="reason in reasons" :key="reason.name" class="relative pl-12">
                <dt class="inline font-semibold text-foreground">
                  <icon
                    :name="reason.icon"
                    class="absolute left-1 top-1 text-4xl text-primary"
                    aria-hidden="true"
                  />
                  {{ reason.name }}
                </dt>
                {{ " " }}
                <dd class="inline">
                  {{ reason.description }}
                </dd>
              </div>
            </dl>
          </div>
        </div>

        <div class="hidden lg:block sm:px-6 lg:px-0">
          <div
            class="relative isolate overflow-hidden rounded-3xl bg-muted px-6 pt-8 sm:mx-auto sm:max-w-2xl sm:pt-16 sm:pr-0 sm:pl-16 lg:mx-0 lg:max-w-none"
          >
            <div
              class="absolute -inset-y-px -left-3 -z-10 w-full origin-bottom-left skew-x-[-30deg] bg-foreground/10 opacity-50 ring-1 ring-border"
              aria-hidden="true"
            />
            <div class="mx-auto max-w-2xl sm:mx-0 sm:max-w-none">
              <!-- Replace with a data-leak / broker themed visual (or a real UI shot) -->
              <img
                src="/assets/images/feature-01.light.png"
                :alt="t('reasons.image_alt')"
                width="998"
                height="1080"
                class="block -mb-12 w-xl max-w-none rounded-tl-xl bg-background ring-1 ring-border/50 lg:w-3xl dark:hidden"
              />
              <img
                src="/assets/images/feature-01.dark.png"
                :alt="t('reasons.image_alt')"
                width="998"
                height="1080"
                class="hidden -mb-12 w-xl max-w-none rounded-tl-xl bg-background ring-1 ring-border/50 lg:w-3xl dark:block"
              />
            </div>
            <div
              class="pointer-events-none absolute inset-0 ring-1 ring-border/50 ring-inset sm:rounded-3xl"
              aria-hidden="true"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  reasons:
    kicker: "Why use an alias mailbox"
    title: "Reduce exposure from leaks and brokers"
    subtitle: "Your email address is a durable identifier. Aliases help limit how far it spreads, and what it can be linked to."
    image_alt: "Illustration related to data exposure and email aliases"
    items:
      - name: "Data brokers stitch identities together."
        description: "A single email address can connect signups, purchases, profiles, and location trails across services."
      - name: "Leaks turn addresses into target lists."
        description: "When a service is breached, email addresses get reused for phishing, credential stuffing, and spam."
      - name: "Stop giving out a permanent identifier."
        description: "Use a different address per service so one exposure doesn’t follow you everywhere."
      - name: "Spot who shared or sold your address."
        description: "If an alias starts receiving unexpected mail, you know which signup led to it."
      - name: "Rotate instead of clean up."
        description: "When an alias gets noisy, disable it and move on. No need to change your primary inbox."
</i18n>
