<script setup lang="ts">
import "vue-sonner/style.css";
import { Toaster } from "../components/ui/sonner";

const { t } = useI18n({
  useScope: "local",
});
</script>

<template>
  <div>
    <nuxt-loading-indicator />
    <SidebarProvider :style="{ '--sidebar-width': '19rem' }">
      <app-sidebar />
      <SidebarInset>
        <header class="flex h-16 shrink-0 items-center gap-2 px-4">
          <SidebarTrigger class="-ml-1" />
          <Separator
            orientation="vertical"
            class="mr-2 data-[orientation=vertical]:h-4"
          />
          <Breadcrumb>
            <BreadcrumbItem>
              <BreadcrumbLink href="/"> Home </BreadcrumbLink>
            </BreadcrumbItem>
          </Breadcrumb>
        </header>
        <div class="flex flex-1 flex-col gap-4 p-4 pt-0">
          <slot />
        </div>
      </SidebarInset>
    </SidebarProvider>
    <Toaster richColors />
  </div>
</template>
