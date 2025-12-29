<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from "vue";

import { Button } from "~/components/ui/button";
import {
  Sheet,
  SheetTrigger,
  SheetContent,
  SheetClose,
} from "~/components/ui/sheet";

type NavItem = { name: string; href: string };

const navigation: NavItem[] = [
  { name: "Features", href: "/#features-01" },
  { name: "FAQ", href: "/#faq" },
  { name: "Security", href: "/#features-02" },
  { name: "Get started", href: "/get-started" },
];

const mobileOpen = ref(false);

const headerRef = ref<HTMLElement | null>(null);
const isSolid = ref(false);

// Toggle a solid header background once the hero gives way to the features section
onMounted(() => {
  const triggerEl = document.getElementById("header-background");
  if (!triggerEl) {
    return;
  }

  const updateSolidState = () => {
    const headerHeight = headerRef.value?.offsetHeight ?? 0;
    const triggerTop = triggerEl.getBoundingClientRect().top + window.scrollY;
    isSolid.value = window.scrollY + headerHeight >= triggerTop;
  };

  const handleScroll = () => updateSolidState();
  const handleResize = () => updateSolidState();

  updateSolidState();

  window.addEventListener("scroll", handleScroll, { passive: true });
  window.addEventListener("resize", handleResize);

  onBeforeUnmount(() => {
    window.removeEventListener("scroll", handleScroll);
    window.removeEventListener("resize", handleResize);
  });
});
</script>

<template>
  <header
    ref="headerRef"
    :class="[
      'fixed inset-x-0 top-0 z-50 transition-colors duration-300',
      isSolid
        ? 'border-b border-neutral-200/60 bg-white/90 backdrop-blur supports-[backdrop-filter]:bg-white/70 dark:border-neutral-800/80 dark:bg-neutral-900/90'
        : 'bg-transparent',
    ]"
  >
    <nav
      class="mx-auto flex max-w-7xl items-center justify-between px-6 py-4 lg:px-8"
      aria-label="Global"
    >
      <!-- Logo -->
      <div class="flex flex-1">
        <NuxtLink to="/" class="-m-1.5 flex items-center gap-2 p-1.5">
          <span class="sr-only">Nullbox</span>

          <img
            src="/assets/logo.light.svg"
            alt="Nullbox Logo"
            class="block h-12 w-auto dark:hidden"
          />
          <img
            src="/assets/logo.dark.svg"
            alt="Nullbox Logo"
            class="hidden h-12 w-auto dark:block"
          />
        </NuxtLink>
      </div>

      <!-- Mobile menu -->
      <div class="flex lg:hidden">
        <Sheet v-model:open="mobileOpen">
          <SheetTrigger as-child>
            <Button variant="ghost" size="icon" class="text-foreground">
              <span class="sr-only">Open main menu</span>
              <Icon name="lucide:menu" class="size-6" />
            </Button>
          </SheetTrigger>

          <SheetContent side="right" class="w-full sm:max-w-sm">
            <div class="flex items-center justify-between">
              <NuxtLink
                to="/"
                class="flex items-center gap-2"
                @click="mobileOpen = false"
              >
                <span class="sr-only">Nullbox</span>

                <img
                  src="/assets/logo.light.svg"
                  alt="Nullbox Logo"
                  class="block h-12 w-auto dark:hidden"
                />
                <img
                  src="/assets/logo.dark.svg"
                  alt="Nullbox Logo"
                  class="hidden h-12 w-auto dark:block"
                />
              </NuxtLink>
            </div>

            <div class="mt-8 space-y-2">
              <SheetClose v-for="item in navigation" :key="item.name" as-child>
                <a
                  :href="item.href"
                  class="block rounded-lg px-3 py-2 text-base font-semibold text-foreground hover:bg-muted"
                >
                  {{ item.name }}
                </a>
              </SheetClose>
            </div>

            <Button class="w-48 mx-auto" as-child>
              <NuxtLink to="https://app.nullbox.email" external>
                Get started
              </NuxtLink>
            </Button>
          </SheetContent>
        </Sheet>
      </div>

      <!-- Desktop nav -->
      <div class="hidden lg:flex lg:gap-x-10">
        <a
          v-for="item in navigation"
          :key="item.name"
          :href="item.href"
          class="text-base font-semibold hover:text-foreground"
        >
          {{ item.name }}
        </a>
      </div>

      <!-- Desktop CTA -->
      <div class="hidden lg:flex lg:flex-1 lg:justify-end">
        <Button variant="secondary" as-child>
          <NuxtLink to="https://app.nullbox.email" external> Log in </NuxtLink>
        </Button>
      </div>
    </nav>
  </header>
</template>
