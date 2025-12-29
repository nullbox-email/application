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

const tab = ref<"hourly" | "daily">("daily");
</script>

<template>
  <div>
    <page-heading :title="t('page.title')" />

    <Tabs v-model="tab" :unmount-on-hide="true" class="mt-6">
      <TabsList>
        <TabsTrigger value="daily"> Last 30 days </TabsTrigger>
        <TabsTrigger value="hourly"> Last 24 hours </TabsTrigger>
      </TabsList>
      <TabsContent value="daily">
        <dashboard-activity
          :active="tab === 'daily'"
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
      </TabsContent>
      <TabsContent value="hourly">
        <dashboard-activity
          :active="tab === 'hourly'"
          :mailbox-id="mailbox.id"
          :number="24"
          type="Hourly"
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
      </TabsContent>
    </Tabs>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  page:
    title: Getting started with your mailbox

  chart:
    series:
      total: Total
      forwarded: Forwarded
      dropped: Dropped
      quarantined: Quarantined
      delivered: Delivered
      failed: Failed
</i18n>
