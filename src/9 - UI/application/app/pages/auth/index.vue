<script setup lang="ts">
definePageMeta({
  layout: "focus",
});

const { t } = useI18n({
  useScope: "local",
});

onMounted(() => {
  setTimeout(async () => {
    await navigateTo("/");
  }, 1500);
});
</script>

<template>
  <Card class="w-105">
    <CardHeader>
      <img
        src="/assets/logo.light.svg"
        alt="Nullbox Logo"
        class="block h-24 mt-4 w-auto dark:hidden"
      />
      <img
        src="/assets/logo.dark.svg"
        alt="Nullbox Logo"
        class="hidden h-24 mt-4 w-auto dark:block"
      />
    </CardHeader>
    <CardContent class="grid gap-4 mb-4">
      <div class="text-center">
        <h3 class="text-xl font-semibold">{{ t("title") }}</h3>
        <p class="text-muted-foreground">{{ t("description") }}</p>
      </div>
    </CardContent>
  </Card>
</template>

<i18n lang="yaml" scope="global">
en:
  title: We're signing you in
  description: Please wait while we sign you in
</i18n>
