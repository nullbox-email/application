<script setup lang="ts">
import { defineComponent, h } from "vue";

const navigation = [
  {
    name: "GitHub",
    href: "https://github.com/nullbox-email",
    icon: "carbon:logo-github",
  },
  {
    name: "Discord",
    href: "https://discord.gg/bRP9xfa7",
    icon: "carbon:logo-discord",
  },
];

const currentYear = new Date().getFullYear();
</script>

<template>
  <footer class="bg-background">
    <div
      class="mx-auto max-w-7xl px-6 py-12 md:flex md:items-center md:justify-between lg:px-8"
    >
      <div class="flex justify-center gap-x-6 md:order-2">
        <nuxt-link
          v-for="item in navigation"
          :key="item.name"
          :to="item.href"
          target="_blank"
          external
          class="text-foreground hover:text-primary"
        >
          <span class="sr-only">{{ item.name }}</span>
          <icon :name="item.icon" class="text-2xl" aria-hidden="true" />
        </nuxt-link>
      </div>
      <p class="mt-8 text-center text-foreground md:order-1 md:mt-0">
        &copy; {{ currentYear }} Nullbox. All rights reserved.
      </p>
    </div>
  </footer>
</template>
