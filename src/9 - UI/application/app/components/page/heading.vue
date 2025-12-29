<script setup lang="ts">
interface PageHeading {
  title: string;
}

defineProps<PageHeading>();
</script>

<template>
  <div class="md:flex md:items-center md:justify-between">
    <div class="min-w-0 flex-1">
      <h2
        class="text-2xl/7 font-bold sm:truncate sm:text-3xl sm:tracking-tight"
      >
        {{ title }}
      </h2>
    </div>
    <div class="mt-4 flex md:mt-0 md:ml-4">
      <slot />
    </div>
  </div>
</template>
