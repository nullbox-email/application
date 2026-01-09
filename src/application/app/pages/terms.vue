<script setup lang="ts">
definePageMeta({
  layout: "legal",
  auth: false,
});

const { t } = useI18n({
  useScope: "local",
});
</script>

<template>
  <div class="flex gap-4">
    <!-- Left column: back button -->
    <div class="pt-1">
      <Button
        variant="link"
        class="text-3xl cursor-pointer p-0"
        as-child
        @click="$router.back()"
      >
        <icon name="lucide:arrow-left" />
      </Button>
    </div>

    <!-- Right column: heading + body -->
    <div class="flex-1">
      <h2
        class="text-2xl/7 font-bold sm:truncate sm:text-3xl sm:tracking-tight"
      >
        {{ t("title") }}
      </h2>

      <div>
        <p class="mt-4 text-lg font-semibold">
          {{ t("sections.service.title") }}
        </p>
        <p>{{ t("sections.service.body") }}</p>

        <p class="mt-4 text-lg font-semibold">
          {{ t("sections.acceptableUse.title") }}
        </p>
        <p>{{ t("sections.acceptableUse.body") }}</p>

        <p class="mt-4 text-lg font-semibold">
          {{ t("sections.account.title") }}
        </p>
        <p>{{ t("sections.account.body") }}</p>

        <p class="mt-4 text-lg font-semibold">
          {{ t("sections.messageHandling.title") }}
        </p>
        <p>{{ t("sections.messageHandling.body") }}</p>

        <p class="mt-4 text-lg font-semibold">
          {{ t("sections.serviceChanges.title") }}
        </p>
        <p>{{ t("sections.serviceChanges.body") }}</p>

        <p class="mt-4 text-lg font-semibold">
          {{ t("sections.liability.title") }}
        </p>
        <p>{{ t("sections.liability.body") }}</p>

        <p class="mt-4 text-lg font-semibold">
          {{ t("sections.termination.title") }}
        </p>
        <p>{{ t("sections.termination.body") }}</p>

        <p class="mt-4 text-lg font-semibold">
          {{ t("sections.governingLaw.title") }}
        </p>
        <p>{{ t("sections.governingLaw.body") }}</p>

        <p class="mt-4 text-lg font-semibold">
          {{ t("sections.contact.title") }}
        </p>
        <p>{{ t("sections.contact.body") }}</p>
      </div>
    </div>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  title: Terms of Service
  sections:
    service:
      title: "1. Service Description"
      body: This service provides email-relay functionality, including aliasing, forwarding, filtering, and optional quarantine features. The service is offered on an “as is” and “as available” basis.
    acceptableUse:
      title: "2. Acceptable Use"
      body: You agree not to use the service for unlawful activity, abuse, harassment, spam, or any action that may disrupt or degrade the service for others. You are responsible for all activity conducted through your aliases and for ensuring that forwarded content complies with applicable laws.
    account:
      title: "3. Account Responsibilities"
      body: You must maintain accurate account information and keep your credentials secure. You are responsible for any actions taken under your account.
    messageHandling:
      title: "4. Message Handling"
      body: We process only the data necessary to relay or quarantine emails. Messages may be temporarily stored solely for operational reasons (e.g., spam checks, quarantine review). Once processed, messages are forwarded or deleted according to service logic. We do not read, mine, or analyze your content beyond what is required for basic functionality and security.
    serviceChanges:
      title: "5. Service Changes"
      body: We may update, modify, or discontinue features as needed to maintain or improve the service. Material changes will be communicated in advance when reasonable.
    liability:
      title: "6. Liability"
      body: We are not liable for lost, delayed, or misdelivered messages, data loss, downtime, security incidents originating outside our systems, or any damages arising from use of the service. Your use of the service is at your own risk.
    termination:
      title: "7. Termination"
      body: We may suspend or terminate access for violation of these Terms or misuse of the service. You may stop using the service at any time.
    governingLaw:
      title: "8. Governing Law"
      body: These Terms are governed by the laws applicable in the service operator’s jurisdiction unless otherwise required by local law.
    contact:
      title: "9. Contact"
      body: You may contact us with questions about these Terms or for clarification regarding your use of the service.
</i18n>
