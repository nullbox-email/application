<script setup lang="ts">
const { t } = useI18n({ useScope: "local" });

useHead({
  title: t("meta.title"),
  titleTemplate: "%s",
});
</script>

<template>
  <site-header />

  <launch-pricing />
  <pricing disabled />
</template>

<i18n lang="yaml" scope="local">
en:
  meta:
    title: "Nullbox"
</i18n>
