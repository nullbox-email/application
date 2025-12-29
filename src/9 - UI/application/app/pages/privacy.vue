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
        <p class="mt-4">{{ t("p1") }}</p>
        <p class="mt-4">{{ t("p2") }}</p>
        <p class="mt-4">{{ t("p3") }}</p>
        <p class="mt-4">{{ t("p4") }}</p>
        <p class="mt-4">{{ t("p5") }}</p>
        <p class="mt-4">{{ t("p6") }}</p>
      </div>
    </div>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  title: Privacy Policy
  p1: This service is designed to provide private, reliable email-relay functionality with minimal data processing. We collect and process only the information required to operate the service, maintain security, prevent abuse, and deliver messages as intended.
  p2: We do not sell, rent, or share your personal data with third parties for marketing or commercial purposes. We do not use your data for profiling or advertising.
  p3: Email metadata and message content are handled solely for the purpose of routing, forwarding, or quarantining messages as part of the relay service. Data is retained only as long as required for these functions, after which it is deleted according to our retention policy. Suspicious or quarantined messages are stored temporarily until you review them.
  p4: Your data never leaves our control except when forwarding messages to your designated inbox provider during normal relay operations.
  p5: We may collect limited technical data (such as logs, IP addresses, or usage metrics) strictly for operational diagnostics, security, fraud prevention, and service performance. This data is minimized wherever possible.
  p6: By using the service, you agree to this privacy policy. You may contact us at any time for questions or to request clarification about how your data is handled.
</i18n>
