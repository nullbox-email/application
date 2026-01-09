<script setup lang="ts">
const { t } = useI18n({ useScope: "local" });

const includedFeatures = computed(() => [
  t("features.mailboxes_unlimited"),
  t("features.aliases_unlimited"),
  t("features.bandwidth_gb", { count: 100 }),
  t("features.support_response_hours", { hours: 24 }),
]);
</script>

<template>
  <section id="get-started" class="bg-background py-24 sm:py-32">
    <div class="mx-auto max-w-7xl px-6 lg:px-8">
      <div class="mx-auto max-w-4xl sm:text-center">
        <h2
          class="text-5xl font-semibold tracking-tight text-balance text-foreground sm:text-6xl"
        >
          {{ t("section.title") }}
        </h2>

        <p
          class="mx-auto mt-6 max-w-2xl text-lg font-medium text-pretty text-muted-foreground sm:text-xl/8"
        >
          {{ t("section.subtitle") }}
        </p>
      </div>

      <div
        class="mx-auto mt-16 max-w-2xl rounded-3xl ring-1 ring-border sm:mt-20 lg:mx-0 lg:flex lg:max-w-none"
      >
        <div class="p-8 sm:p-10 lg:flex-auto">
          <h3 class="text-3xl font-semibold tracking-tight text-foreground">
            {{ t("plan.name") }}
          </h3>

          <p class="mt-2 text-sm/6 font-semibold text-primary">
            {{ t("plan.tagline") }}
          </p>

          <p class="mt-6 text-base/7 text-muted-foreground">
            {{ t("plan.description") }}
          </p>

          <div class="mt-10 flex items-center gap-x-4">
            <h4 class="flex-none text-sm/6 font-semibold text-primary">
              {{ t("labels.whats_included") }}
            </h4>
            <div class="h-px flex-auto bg-border" />
          </div>

          <ul
            role="list"
            class="mt-8 grid grid-cols-1 gap-4 text-sm/6 text-muted-foreground sm:grid-cols-2 sm:gap-6"
          >
            <li
              v-for="feature in includedFeatures"
              :key="feature"
              class="flex gap-x-3"
            >
              <icon
                name="heroicons:check"
                class="text-xl flex-none text-primary"
                aria-hidden="true"
              />
              {{ feature }}
            </li>
          </ul>
        </div>

        <div class="-mt-2 p-2 lg:mt-0 lg:w-full lg:max-w-md lg:shrink-0">
          <div
            class="rounded-2xl bg-muted py-10 text-center ring-1 ring-border lg:flex lg:flex-col lg:justify-center lg:py-16"
          >
            <div class="mx-auto max-w-xs px-8">
              <p class="text-base font-semibold text-muted-foreground">
                {{ t("price.tagline") }}
              </p>

              <p class="mt-6 flex items-baseline justify-center gap-x-2">
                <span
                  class="text-5xl font-semibold tracking-tight text-foreground"
                >
                  {{ t("price.amount") }}
                </span>
              </p>

              <p class="mt-2 text-sm/6 font-semibold text-muted-foreground">
                {{ t("price.suffix") }}
              </p>

              <nuxt-link
                to="https://app.nullbox.email"
                external
                class="mt-10 inline-flex h-10 w-full items-center justify-center rounded-md bg-primary px-4 text-sm font-semibold text-primary-foreground shadow-sm transition-colors hover:bg-primary/90 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 focus-visible:ring-offset-background"
              >
                {{ t("cta.get_access") }}
              </nuxt-link>

              <p class="mt-6 text-xs/5 text-muted-foreground">
                {{
                  t("price.note", { bandwidth: 100 })
                }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<i18n lang="yaml" scope="local">
en:
  section:
    title: "Early access"
    subtitle: "Pro features for early adopters. Billing is not available yet."

  plan:
    name: "Early access"
    tagline: "Limited early adopter offer"
    description: "Full Pro features during early access."

  price:
    tagline: "Early access"
    amount: "Free"
    suffix: "until billing is available"
    note: "Includes unlimited mailboxes and aliases, plus {bandwidth} GB/month bandwidth. When billing is available, you get 3 months of Pro included."

  cta:
    get_access: "Get early access"

  labels:
    whats_included: "What’s included"

  features:
    mailboxes_unlimited: "Unlimited mailboxes"
    aliases_unlimited: "Unlimited aliases"
    bandwidth_gb: "{count} GB/month bandwidth"
    support_response_hours: "Support response within {hours} hours"
</i18n>
