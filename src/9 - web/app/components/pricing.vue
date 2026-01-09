<script setup lang="ts">
const { t } = useI18n({ useScope: "local" })

const props = withDefaults(
  defineProps<{
    disabled?: boolean
  }>(),
  { disabled: false },
)

const frequencies = [
  { value: "monthly", label: t("frequency.monthly") },
  { value: "annually", label: t("frequency.annually") },
]

const tiers = computed(() => [
  {
    name: t("tiers.free.name"),
    id: "tier-free",
    href: "#",
    price: { monthly: t("tiers.free.price.monthly"), annually: t("tiers.free.price.annually") },
    description: t("tiers.free.description"),
    features: [
      t("features.mailboxes", 1),
      t("features.aliases", 10),
      t("features.bandwidth_mb", { count: 100 }),
      t("features.support_response_hours", { hours: 48 }),
    ],
    featured: false,
    showSuffix: true,
  },
  {
    name: t("tiers.plus.name"),
    id: "tier-plus",
    href: "#",
    price: { monthly: t("tiers.plus.price.monthly"), annually: t("tiers.plus.price.annually") },
    description: t("tiers.plus.description"),
    features: [
      t("features.mailboxes", 5),
      t("features.aliases", 50),
      t("features.bandwidth_gb", { count: 1 }),
      t("features.support_response_hours", { hours: 48 }),
    ],
    featured: true,
    showSuffix: true,
  },
  {
    name: t("tiers.pro.name"),
    id: "tier-pro",
    href: "#",
    price: { monthly: t("tiers.pro.price.monthly"), annually: t("tiers.pro.price.annually") },
    description: t("tiers.pro.description"),
    features: [
      t("features.mailboxes_unlimited"),
      t("features.aliases_unlimited"),
      t("features.bandwidth_gb", { count: 100 }),
      t("features.support_response_hours", { hours: 24 }),
    ],
    featured: false,
    showSuffix: true,
  },
])
</script>

<template>
  <form class="group/tiers bg-background py-24 sm:py-32">
    <div class="mx-auto max-w-7xl px-6 lg:px-8">
      <div class="mx-auto max-w-4xl text-center">
        <h2 class="text-base/7 font-semibold text-primary">{{ t("section.kicker") }}</h2>
        <p class="mt-2 text-5xl font-semibold tracking-tight text-balance text-foreground sm:text-6xl">
          {{ t("section.title") }}
        </p>
      </div>

      <p class="mx-auto mt-6 max-w-2xl text-center text-lg font-medium text-pretty text-muted-foreground sm:text-xl/8">
        {{ props.disabled ? t("section.subtitle_disabled") : t("section.subtitle") }}
      </p>

      <!-- Only show the frequency toggle when selectable -->
      <div v-if="!props.disabled" class="mt-16 flex justify-center">
        <fieldset :aria-label="t('frequency.aria_label')">
          <div
            class="grid grid-cols-2 gap-x-1 rounded-full bg-muted p-1 text-center text-xs/5 font-semibold text-muted-foreground ring-1 ring-inset ring-border"
          >
            <label class="group relative rounded-full px-2.5 py-1 has-checked:bg-primary">
              <input
                type="radio"
                name="frequency"
                value="monthly"
                checked=""
                class="absolute inset-0 appearance-none rounded-full"
              />
              <span class="group-has-checked:text-primary-foreground">{{ frequencies[0].label }}</span>
            </label>

            <label class="group relative rounded-full px-2.5 py-1 has-checked:bg-primary">
              <input
                type="radio"
                name="frequency"
                value="annually"
                class="absolute inset-0 appearance-none rounded-full"
              />
              <span class="group-has-checked:text-primary-foreground">{{ frequencies[1].label }}</span>
            </label>
          </div>
        </fieldset>
      </div>

      <div class="isolate mx-auto mt-10 grid max-w-md grid-cols-1 gap-8 lg:mx-0 lg:max-w-none lg:grid-cols-3">
        <div
          v-for="tier in tiers"
          :key="tier.id"
          class="group/tier rounded-3xl bg-card p-8 text-card-foreground ring-1 ring-border data-featured:ring-2 data-featured:ring-primary xl:p-10"
          :class="props.disabled ? 'opacity-70' : ''"
          :data-featured="tier.featured ? 'true' : undefined"
        >
          <div class="flex items-center justify-between gap-x-4">
            <h3 :id="tier.id" class="text-lg/8 font-semibold text-foreground group-data-featured/tier:text-primary">
              {{ tier.name }}
            </h3>

            <!-- Badge: show "Coming soon" when disabled, otherwise "Most popular" -->
            <p
              v-if="props.disabled"
              class="rounded-full bg-muted px-2.5 py-1 text-xs/5 font-semibold text-muted-foreground"
            >
              {{ t("labels.coming_soon") }}
            </p>

            <p
              v-else
              class="rounded-full bg-primary/10 px-2.5 py-1 text-xs/5 font-semibold text-primary group-not-data-featured/tier:hidden"
            >
              {{ t("labels.most_popular") }}
            </p>
          </div>

          <p class="mt-4 text-sm/6 text-muted-foreground">{{ tier.description }}</p>

          <!-- Prices:
               - when enabled: use the frequency toggle
               - when disabled: show both monthly + annually for reference -->
          <p
            class="mt-6 flex items-baseline gap-x-1"
            :class="props.disabled ? '' : 'group-not-has-[[name=frequency][value=monthly]:checked]/tiers:hidden'"
          >
            <span class="text-4xl font-semibold tracking-tight text-foreground">
              {{ tier.price.monthly }}
            </span>
            <span v-if="tier.showSuffix" class="text-sm/6 font-semibold text-muted-foreground">
              {{ t("price_suffix.monthly") }}
            </span>
          </p>

          <p
            class="flex items-baseline gap-x-1"
            :class="
              props.disabled
                ? 'mt-2'
                : 'mt-6 group-not-has-[[name=frequency][value=annually]:checked]/tiers:hidden'
            "
          >
            <span class="text-4xl font-semibold tracking-tight text-foreground">
              {{ tier.price.annually }}
            </span>
            <span v-if="tier.showSuffix" class="text-sm/6 font-semibold text-muted-foreground">
              {{ t("price_suffix.annually") }}
            </span>
          </p>

          <!-- CTA: disabled state -->
          <a
            v-if="!props.disabled"
            :href="tier.href"
            :aria-describedby="tier.id"
            class="mt-6 inline-flex h-10 w-full items-center justify-center rounded-md px-4 text-sm font-semibold transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 focus-visible:ring-offset-background"
            :class="
              tier.featured
                ? 'bg-primary text-primary-foreground hover:bg-primary/90'
                : 'border border-input bg-background text-foreground hover:bg-accent hover:text-accent-foreground'
            "
          >
            {{ t("cta.choose_plan") }}
          </a>

          <button
            v-else
            type="button"
            disabled
            :aria-describedby="tier.id"
            class="mt-6 inline-flex h-10 w-full items-center justify-center rounded-md border border-input bg-background px-4 text-sm font-semibold text-muted-foreground opacity-70 cursor-not-allowed"
          >
            {{ t("cta.coming_soon") }}
          </button>

          <ul role="list" class="mt-8 space-y-3 text-sm/6 text-muted-foreground xl:mt-10">
            <li v-for="feature in tier.features" :key="feature" class="flex gap-x-3">
              <icon name="heroicons:check" class="text-xl flex-none text-primary" aria-hidden="true" />
              {{ feature }}
            </li>
          </ul>
        </div>
      </div>
    </div>
  </form>
</template>

<i18n lang="yaml" scope="local">
en:
  section:
    kicker: "Pricing"
    title: "Plans that fit your usage"
    subtitle: "Pick monthly or annual billing."
    subtitle_disabled: "Paid plans are coming in the next few weeks. Prices shown here are for reference."

  frequency:
    aria_label: "Billing frequency"
    monthly: "Monthly"
    annually: "Annually"

  price_suffix:
    monthly: "/month"
    annually: "/year"

  labels:
    most_popular: "Most popular"
    coming_soon: "Coming soon"

  cta:
    choose_plan: "Choose plan"
    coming_soon: "Not available yet"

  tiers:
    free:
      name: "Free"
      price:
        monthly: "$0"
        annually: "$0"
      description: "For trying things out and light personal use."
    plus:
      name: "Plus"
      price:
        monthly: "$3"
        annually: "$30"
      description: "For regular use with more room to grow."
    pro:
      name: "Pro"
      price:
        monthly: "$5"
        annually: "$50"
      description: "For heavy usage and advanced workflows."

  features:
    mailboxes: "{count} mailbox | {count} mailboxes"
    aliases: "{count} alias | {count} aliases"
    mailboxes_unlimited: "Unlimited mailboxes"
    aliases_unlimited: "Unlimited aliases"
    bandwidth_mb: "{count} MB/month bandwidth"
    bandwidth_gb: "{count} GB/month bandwidth"
    support_response_hours: "Support response within {hours} hours"
</i18n>
