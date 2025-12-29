<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from "vue";

definePageMeta({
  layout: "focus",
});

useHead({
  title: "Ready for lift-off",
});

const { t } = useI18n({
  useScope: "local",
});

const { clear } = useUserSession();

const progress = ref(0);
let intervalId: ReturnType<typeof setInterval> | null = null;

const showManualLink = ref(false);

const numberOfSeconds = 10;

onMounted(async () => {
  await clear();

  // Progress: 0 -> 100 over 10s (every 100ms)
  intervalId = setInterval(() => {
    if (progress.value >= 100) {
      progress.value = 100;

      setTimeout(() => {
        showManualLink.value = true;
      }, 2500);
      if (intervalId) {
        clearInterval(intervalId);
        intervalId = null;
      }
      return;
    }

    progress.value += 1;
  }, numberOfSeconds * 10);

  // Hard redirect to / after 10s
  setTimeout(async () => {
    if (intervalId) {
      clearInterval(intervalId);
      intervalId = null;
    }

    await navigateTo("/", { replace: true });
  }, numberOfSeconds * 1000);
});

onUnmounted(() => {
  if (intervalId) {
    clearInterval(intervalId);
    intervalId = null;
  }
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
        <Progress :model-value="progress" class="h-3 mt-2" />

        <h3 class="mt-4 text-lg font-semibold">
          {{ t("title") }}
        </h3>
        <p class="mt-1 text-muted-foreground">
          {{ t("description") }}
        </p>

        <p v-if="showManualLink" class="mt-4 text-sm text-muted-foreground">
          {{ t("fallbackBody") }}
          <nuxt-link
            to="/"
            class="underline underline-offset-4 hover:text-primary"
          >
            {{ t("fallbackLink") }}
          </nuxt-link>
        </p>
      </div>
    </CardContent>
  </Card>
</template>

<i18n lang="yaml" scope="global">
en:
  title: Getting things ready
  description: This should only take a moment.
  fallbackBody: Not seeing your dashboard?
  fallbackLink: click here.
</i18n>
