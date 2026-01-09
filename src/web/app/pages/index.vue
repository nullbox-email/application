<script setup lang="ts">
const { t } = useI18n({ useScope: "local" });

useHead({
  title: t("title"),
  titleTemplate: "%s",
});
</script>

<template>
  <div class="scroll-smooth">
    <hero />
    <launch-pricing />
    <features01 />
    <cta-01 learnMoreUrl="#faq" />
    <faq />
    <cta-02 learnMoreUrl="#features-02" />
    <features02 />
    <cta-03 learnMoreUrl="#features-03" />
    <features03 />
    <cta-04 />
    <web-footer />
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  title: "Nullbox"
</i18n>