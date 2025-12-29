<script setup lang="ts">
const config = useRuntimeConfig();
</script>

<template>
  <div class="p-1">
    <span> {{ config.public.version }} </span>
  </div>
</template>
