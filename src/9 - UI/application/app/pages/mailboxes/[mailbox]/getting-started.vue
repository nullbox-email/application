<script setup lang="ts">
import type { GetMailboxResponse } from "~/types/dto/mailbox/get-mailbox-response";

const { t } = useI18n({
  useScope: "local",
});

const route = useRoute();

const { get } = useMailboxes();
const mailbox = (await get("v1", {
  id: route.params.mailbox as string,
})) as GetMailboxResponse;
</script>

<template>
  <div>
    <page-heading :title="t('page.title')" />

    <mailbox-getting-started :mailbox="mailbox" class="mt-6" />
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  page:
    title: Getting started with your mailbox
</i18n>
