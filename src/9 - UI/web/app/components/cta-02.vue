<script setup lang="ts">
const { t } = useI18n({ useScope: "local" })
</script>

<template>
  <div class="overflow-hidden bg-primary/35 py-32">
    <div class="mx-auto max-w-7xl px-6 lg:flex lg:px-8">
      <div
        class="mx-auto grid max-w-2xl grid-cols-1 gap-x-12 gap-y-16 lg:mx-0 lg:max-w-none lg:min-w-full lg:flex-none lg:gap-y-8"
      >
        <div class="lg:col-end-1 lg:w-full lg:max-w-lg lg:pb-8">
          <h2 class="text-4xl font-semibold tracking-tight text-foreground sm:text-5xl">
            {{ t("ctaBlock.title") }}
          </h2>

          <p class="mt-6 text-xl/8 text-muted-foreground">
            {{ t("ctaBlock.lede") }}
          </p>

          <p class="mt-6 text-base/7 text-muted-foreground">
            {{ t("ctaBlock.body") }}
          </p>

          <div class="mt-10 flex items-center gap-x-4">
            <Button as-child>
              <nuxt-link to="/get-started" external>
                {{ t("ctaBlock.primary") }}
              </nuxt-link>
            </Button>

            <Button variant="secondary" as-child>
              <nuxt-link to="#features-02">
                {{ t("ctaBlock.secondary") }}
              </nuxt-link>
            </Button>
          </div>

          <p class="mt-6 text-sm text-muted-foreground">
            {{ t("ctaBlock.note") }}
          </p>
        </div>

        <div class="flex flex-wrap items-start justify-end gap-6 sm:gap-8 lg:contents">
          <div class="w-0 flex-auto lg:ml-auto lg:w-auto lg:flex-none lg:self-end">
            <img
              src="https://images.unsplash.com/photo-1670272502246-768d249768ca?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1152&q=80"
              :alt="t('ctaBlock.images.primaryAlt')"
              class="aspect-7/5 w-148 max-w-none rounded-2xl bg-muted object-cover max-sm:w-120"
            />
          </div>

          <div
            class="contents lg:col-span-2 lg:col-end-2 lg:ml-auto lg:flex lg:w-148 lg:items-start lg:justify-end lg:gap-x-8"
          >
            <div class="order-first flex w-64 flex-none justify-end self-end max-sm:w-40 lg:w-auto">
              <img
                src="https://images.unsplash.com/photo-1605656816944-971cd5c1407f?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=768&h=604&q=80"
                :alt="t('ctaBlock.images.secondaryAlt')"
                class="aspect-4/3 w-[24rem] max-w-none flex-none rounded-2xl bg-muted object-cover"
              />
            </div>

            <div class="flex w-96 flex-auto justify-end lg:w-auto lg:flex-none">
              <img
                src="https://images.unsplash.com/photo-1568992687947-868a62a9f521?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1152&h=842&q=80"
                :alt="t('ctaBlock.images.tertiaryAlt')"
                class="aspect-7/5 w-148 max-w-none flex-none rounded-2xl bg-muted object-cover max-sm:w-120"
              />
            </div>

            <div class="hidden sm:block sm:w-0 sm:flex-auto lg:w-auto lg:flex-none">
              <img
                src="https://images.unsplash.com/photo-1612872087720-bb876e2e67d1?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=768&h=604&q=80"
                :alt="t('ctaBlock.images.quaternaryAlt')"
                class="aspect-4/3 w-[24rem] max-w-none rounded-2xl bg-muted object-cover"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  ctaBlock:
    title: Ready to take your mailbox back?
    lede: Stop giving away your real email address. Create aliases, route mail wherever you want, and shut down spam the moment it starts.
    body: Use one alias per site, keep your inbox clean, and stay in control. When something gets noisy, disable the alias—your primary address stays private.
    primary: Get started
    secondary: Learn more
    note: No credit card required.
    images:
      primaryAlt: Inbox control
      secondaryAlt: Alias routing
      tertiaryAlt: Clean inbox
      quaternaryAlt: Email privacy
</i18n>
