<script setup lang="ts">
import 'vue-sonner/style.css'
import { Toaster } from '@/components/ui/sonner'
</script>

<template>
  <div>
    <nuxt-loading-indicator />
    <slot />
    <Toaster richColors />
  </div>
</template>
