<script setup lang="ts">
import 'vue-sonner/style.css'
import { Toaster } from '@/components/ui/sonner'
</script>

<template>
  <div class="flex h-full min-h-svh p-6 relative" style="background: url('/assets/background/focus.svg') no-repeat center center fixed; background-size: cover;">
    <nuxt-loading-indicator />
    <div class="relative w-fit m-auto">
      <slot />
    </div>
    <Toaster richColors />
  </div>
</template>

