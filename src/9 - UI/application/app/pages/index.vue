<script setup lang="ts">
const { t } = useI18n({ useScope: "local" });
const { user } = useUserSession();

const { getAll, get } = useMailboxes();
const mailboxes = await getAll("v1");

const mailbox = await get("v1", {
  id: `${mailboxes[0].routingKey}.${mailboxes[0].domain}`,
});
</script>

<template>
  <div>
    <page-heading :title="t('title', { name: user.name })" />

    <mailbox-getting-started
      v-if="mailbox && mailbox.aliases?.length === 0"
      :mailbox="mailbox"
      class="mt-6"
    />

    <dashboard-activity
      active
      :number="30"
      type="Daily"
      :fallback-error="t('submit.error')"
      :series="[
        {
          key: 'dropped',
          label: t('chart.series.dropped'),
          color: 'var(--destructive)',
          fill: { id: 'fillDropped' },
        },
        {
          key: 'quarantined',
          label: t('chart.series.quarantined'),
          color: 'var(--warning)',
          fill: { id: 'fillQuarantined' },
        },
        {
          key: 'forwarded',
          label: t('chart.series.forwarded'),
          color: 'var(--chart-2)',
          fill: { id: 'fillForwarded' },
        },
      ]"
    />
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  title: "Hi {name}"

  chart:
    series:
      total: Total
      forwarded: Forwarded
      dropped: Dropped
      quarantined: Quarantined
      delivered: Delivered
      failed: Failed
</i18n>
