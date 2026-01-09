<script setup lang="ts">
import type { GetResponse } from "~/types/dto/mailboxes/get-response";

const { t } = useI18n({
  useScope: "local",
});

const route = useRoute();

const { get } = useMailboxes();
const mailbox = (await get("v1", {
  id: route.params.mailbox as string,
})) as GetResponse;
</script>

<template>
  <div>
    <page-heading :title="t('page.title', { mailbox: mailbox.name })">
      <div class="flex items-center space-x-4">
        <Button as-child>
          <nuxt-link :to="`${route.fullPath}/settings`">
            {{ t("actions.edit") }}
          </nuxt-link>
        </Button>
      </div>
    </page-heading>

    <dashboard-activity
      active
      :mailbox-id="mailbox.id"
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
  page:
    title: "{mailbox} activity"

  actions:
    edit: Edit

  chart:
    series:
      total: Total
      forwarded: Forwarded
      dropped: Dropped
      quarantined: Quarantined
      delivered: Delivered
      failed: Failed
</i18n>
