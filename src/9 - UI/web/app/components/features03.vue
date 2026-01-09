<script setup lang="ts">
const { t } = useI18n({ useScope: "local" })

type Item = {
  name: string
  description: string
  icon: string
}

const items = computed<Item[]>(() => [
  {
    name: t("limits.items.no_content_processing.name"),
    description: t("limits.items.no_content_processing.description"),
    icon: t("limits.items.no_content_processing.icon"),
  },
  {
    name: t("limits.items.no_message_storage.name"),
    description: t("limits.items.no_message_storage.description"),
    icon: t("limits.items.no_message_storage.icon"),
  },
  {
    name: t("limits.items.no_profiles.name"),
    description: t("limits.items.no_profiles.description"),
    icon: t("limits.items.no_profiles.icon"),
  },
  {
    name: t("limits.items.no_inbox_access.name"),
    description: t("limits.items.no_inbox_access.description"),
    icon: t("limits.items.no_inbox_access.icon"),
  },
  {
    name: t("limits.items.no_data_sales.name"),
    description: t("limits.items.no_data_sales.description"),
    icon: t("limits.items.no_data_sales.icon"),
  },
  {
    name: t("limits.items.no_ads.name"),
    description: t("limits.items.no_ads.description"),
    icon: t("limits.items.no_ads.icon"),
  },
])
</script>

<template>
  <section id="features-03" class="bg-background py-24 sm:py-32 lg:py-40">
    <div class="mx-auto max-w-7xl px-6 lg:px-8">
      <div class="mx-auto max-w-2xl lg:text-center">
        <h2 class="text-base/7 font-semibold text-primary">
          {{ t("limits.kicker") }}
        </h2>
        <p class="mt-2 text-4xl font-semibold tracking-tight text-pretty text-foreground sm:text-5xl lg:text-balance">
          {{ t("limits.title") }}
        </p>
        <p class="mt-6 text-lg/8 text-muted-foreground">
          {{ t("limits.description") }}
        </p>
      </div>

      <div class="mx-auto mt-16 max-w-2xl sm:mt-20 lg:mt-24 lg:max-w-none">
        <dl class="grid max-w-xl grid-cols-1 gap-x-8 gap-y-16 lg:max-w-none lg:grid-cols-3">
          <div v-for="item in items" :key="item.name" class="flex flex-col">
            <!-- Alignment fix: top-align, icon nudged down slightly -->
            <dt class="flex items-center gap-x-3 text-base/7 font-semibold text-foreground">
              <Icon
                :name="item.icon"
                class="mt-0.5 text-4xl shrink-0 text-primary"
                aria-hidden="true"
              />
              <span>{{ item.name }}</span>
            </dt>

            <dd class="mt-4 text-base/7 text-muted-foreground">
              {{ item.description }}
            </dd>
          </div>
        </dl>
      </div>
    </div>
  </section>
</template>

<i18n lang="yaml" scope="local">
en:
  limits:
    kicker: Operational limits
    title: What we don’t do
    description: "These are deliberate constraints. They reduce exposure and keep the system simple."
    items:
      no_content_processing:
        icon: "solar:inbox-archive-bold-duotone"
        name: No content processing
        description: "We don’t ingest or analyze message content to decide whether to forward or drop."
      no_message_storage:
        icon: "solar:cloud-cross-bold-duotone"
        name: No copy of your mailbox
        description: "We don’t store a copy of forwarded mail as a mailbox you can search or browse."
      no_profiles:
        icon: "solar:user-cross-rounded-bold-duotone"
        name: No identity profiles across aliases
        description: "We don’t use alias activity to build identity profiles across services."
      no_inbox_access:
        icon: "solar:key-square-2-bold-duotone"
        name: No access to your existing inbox
        description: "We don’t ask for OAuth access to Gmail, Outlook, or your email provider."
      no_data_sales:
        icon: "solar:dollar-minimalistic-bold-duotone"
        name: No selling mailbox data
        description: "We don’t sell mailbox data or monetize via data brokerage."
      no_ads:
        icon: "solar:station-minimalistic-bold-duotone"
        name: No advertising model
        description: "We don’t run ads or monetize by targeting based on mailbox activity."
</i18n>
