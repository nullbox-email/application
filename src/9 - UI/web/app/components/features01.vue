<script setup lang="ts">
const { t } = useI18n({ useScope: "local" })

const features = [
  {
    name: t("nullbox_features.items.0.name"),
    description: t("nullbox_features.items.0.description"),
    icon: "solar:shield-keyhole-bold-duotone",
  },
  {
    name: t("nullbox_features.items.1.name"),
    description: t("nullbox_features.items.1.description"),
    icon: "solar:mailbox-bold-duotone",
  },
  {
    name: t("nullbox_features.items.2.name"),
    description: t("nullbox_features.items.2.description"),
    icon: "solar:virus-bold-duotone",
  },
  {
    name: t("nullbox_features.items.3.name"),
    description: t("nullbox_features.items.3.description"),
    icon: "solar:shield-minimalistic-bold-duotone",
  },
  {
    name: t("nullbox_features.items.4.name"),
    description: t("nullbox_features.items.4.description"),
    icon: "solar:videocamera-record-bold-duotone",
  },
]
</script>

<template>
  <div id="features-01" class="overflow-hidden bg-background py-24 sm:py-32 lg:py-40">
    <div class="mx-auto max-w-7xl px-6 lg:px-8">
      <div
        class="mx-auto grid max-w-2xl grid-cols-1 gap-x-8 gap-y-16 sm:gap-y-20 lg:mx-0 lg:max-w-none lg:grid-cols-2"
      >
        <div class="lg:ml-auto lg:pt-4 lg:pl-4">
          <div class="lg:max-w-lg">
            <h2 class="text-sm font-semibold text-foreground/80">
              {{ t("nullbox_features.kicker") }}
            </h2>

            <p
              class="mt-2 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl"
            >
              {{ t("nullbox_features.title") }}
            </p>

            <p class="mt-6 text-lg leading-8 text-muted-foreground">
              {{ t("nullbox_features.subtitle") }}
            </p>

            <dl class="mt-10 max-w-xl space-y-8 text-base leading-7 text-muted-foreground lg:max-w-none">
              <div
                v-for="feature in features"
                :key="feature.name"
                class="relative pl-12"
              >
                <dt class="inline font-semibold text-foreground">
                  <icon
                    :name="feature.icon"
                    class="absolute left-1 top-1 text-4xl text-primary"
                    aria-hidden="true"
                  />
                  {{ feature.name }}
                </dt>
                {{ " " }}
                <dd class="inline">
                  {{ feature.description }}
                </dd>
              </div>
            </dl>
          </div>
        </div>

        <div class="hidden lg:block lg:order-first lg:pr-4">
          <div class="relative isolate overflow-hidden rounded-3xl">
            <img
              src="/assets/images/feature-01.light.png"
              :alt="t('nullbox_features.image_alt')"
              width="998"
              height="1080"
              class="block w-xl max-w-none object-cover lg:w-3xl dark:hidden"
            />
            <img
              src="/assets/images/feature-01.dark.png"
              :alt="t('nullbox_features.image_alt')"
              width="998"
              height="1080"
              class="hidden w-xl max-w-none object-cover lg:w-3xl dark:block"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  nullbox_features:
    kicker: "Nullbox features"
    title: "Privacy that fits into your existing inbox"
    subtitle: "Use aliases by default. Keep your real address out of forms, lists, and leaks."
    image_alt: "Nullbox product screenshot"
    items:
      - name: "Private mailbox domains."
        description: "Each mailbox gets its own domain for aliases. Share addresses freely without exposing your primary inbox."
      - name: "Forwarding to your real inbox."
        description: "Mail is forwarded to your existing email address. You keep your current workflow and add a privacy layer in front."
      - name: "Unexpected sender quarantine."
        description: "Hold messages that don’t match the usual senders for an alias. Helps catch lookalikes, mistargets, and unexpected mail."
      - name: "Alias-level control."
        description: "Create, disable, and rotate aliases as needed. Keep services reachable without keeping the same address forever."
      - name: "Minimal data surface."
        description: "Designed to collect and retain as little as possible. The goal is utility without building an identity profile."
</i18n>
