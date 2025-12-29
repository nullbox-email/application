<script setup lang="ts">
import "vue-sonner/style.css";
import { Toaster } from "@/components/ui/sonner";

const { t } = useI18n({
  useScope: "local",
});
</script>

<template>
  <div>
    <nuxt-loading-indicator />
        <div class="flex flex-1 flex-col gap-4 p-4 pt-0">
          <slot />
        </div>
    <Toaster richColors />
  </div>
</template>
