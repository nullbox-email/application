<script setup lang="ts">
const { t } = useI18n({ useScope: "local" })

type Faq = {
  question: string
  answer: string
}

const faqs = computed<Faq[]>(() => [
  {
    question: t("faq.items.use_cases.question"),
    answer: t("faq.items.use_cases.answer"),
  },
  {
    question: t("faq.items.how.question"),
    answer: t("faq.items.how.answer"),
  },
  {
    question: t("faq.items.spam.question"),
    answer: t("faq.items.spam.answer"),
  },
  {
    question: t("faq.items.replies.question"),
    answer: t("faq.items.replies.answer"),
  },
  {
    question: t("faq.items.privacy.question"),
    answer: t("faq.items.privacy.answer"),
  },
  {
    question: t("faq.items.cost.question"),
    answer: t("faq.items.cost.answer"),
  },
])

const contactOptions = computed(() => [
  {
    name: t("faq.contact.github"),
    href: "https://github.com/nullbox-email/support",
    icon: "carbon:logo-github",
  },
  {
    name: t("faq.contact.discord"),
    href: "https://discord.gg/bRP9xfa7",
    icon: "carbon:logo-discord",
  },
])
</script>

<template>
  <section id="faq" class="bg-background py-24 sm:py-32 lg:py-40">
    <div class="mx-auto max-w-7xl px-6 lg:px-8">
      <div class="mx-auto max-w-2xl lg:max-w-none lg:grid lg:grid-cols-12 lg:gap-8">
        <div class="lg:col-span-5">
          <h2 class="text-3xl font-semibold tracking-tight text-foreground sm:text-4xl">
            {{ t("faq.title") }}
          </h2>

          <p class="mt-4 text-base/7 text-muted-foreground">
            {{ t("faq.lead") }}
          </p>

          <ul class="mt-6 space-y-3">
            <li v-for="item in contactOptions" :key="item.name">
              <NuxtLink
                :to="item.href"
                target="_blank"
                external
                class="inline-flex items-center gap-3 text-foreground hover:text-primary"
              >
                <icon :name="item.icon" class="text-2xl" aria-hidden="true" />
                <span class="text-base font-medium">{{ item.name }}</span>
              </NuxtLink>
            </li>
          </ul>
        </div>

        <div class="mt-10 lg:col-span-7 lg:mt-0">
          <dl class="space-y-10">
            <div v-for="item in faqs" :key="item.question" class="space-y-2">
              <dt class="text-base/7 font-semibold text-foreground">
                {{ item.question }}
              </dt>
              <dd class="text-base/7 text-muted-foreground">
                {{ item.answer }}
              </dd>
            </div>
          </dl>
        </div>
      </div>
    </div>
  </section>
</template>

<i18n lang="yaml">
en:
  faq:
    title: Frequently asked questions
    lead: "Can’t find the answer you’re looking for? Contact us below and we'll be happy to help."
    contact:
      github: GitHub
      discord: Discord
    items:
      cost:
        question: How much does it cost?
        answer: "It’s free for now. When we introduce paid plans, pricing is expected to start around $3/month, with Pro at around $5/month. Early users receive one free year of Pro from the start of monetization. We plan to always offer a free tier."
      how:
        question: How does it work?
        answer: "Incoming mail is handled by a Cloudflare Email Worker. The worker checks alias and mailbox metadata with our API to decide whether to forward or drop the message. Only metadata is logged. Messages that are forwarded are sent directly via a mail-forwarding service and do not pass through our system."
      privacy:
        question: What’s our approach to privacy?
        answer: "We try to collect as little as possible. If we don’t need data to run the service, we try not to store it. The goal is to reduce tracking surface and inbox noise while keeping you in control."
      use_cases:
        question: What is this for, in practice?
        answer: "Use it when you need an email address but don’t want to share your primary inbox — newsletters, sign-ups, online shopping, trials, and forms. It also limits the impact of data breaches by keeping exposure isolated to individual aliases."
      spam:
        question: Can I stop an alias if it starts getting spam?
        answer: "Yes. You can disable or rotate aliases, so spam stays contained to the address that leaked."
      replies:
        question: Will this break replies or verification emails?
        answer: "Verification and transactional emails should forward as expected. Reply support depends on how you send (we aim to keep common flows working)."
</i18n>
