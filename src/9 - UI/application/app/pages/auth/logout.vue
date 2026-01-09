<script setup lang="ts">
definePageMeta({
  layout: "focus",
});

const { t } = useI18n({
  useScope: "local",
});

const { clear } = useUserSession();
const config = useRuntimeConfig();

onMounted(() => {
  setTimeout(async () => {
    await clear();

    const origin = window.location.origin;
    const redirect = encodeURIComponent(`${origin}/`);

    const tenant = config.public.entraExternalTenant;
    const tenantId = config.public.entraExternalTenantId;

    const url = `https://${tenant}.ciamlogin.com/${tenantId}/oauth2/logout?post_logout_redirect_uri=${redirect}`;

    await navigateTo(url, { external: true });
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
  title: Signing you out
  description: Please wait while we sign you out
</i18n>
