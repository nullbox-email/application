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

    <mailbox-getting-started :mailbox="mailbox" class="mt-6" />
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  title: "Hi {name}"
</i18n>
