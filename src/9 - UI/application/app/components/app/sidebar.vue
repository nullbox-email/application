<script setup lang="ts">
import type { SidebarProps } from "../ui/sidebar";

const props = withDefaults(defineProps<SidebarProps>(), {
  variant: "floating",
});

const { t } = useI18n({
  useScope: "local",
});

const data = {
  navMain: [
    {
      title: "Home",
      to: "/",
      icon: "solar:home-smile-bold-duotone",
    },

    {
      title: "Create new mailbox",
      to: "/mailboxes/create",
      icon: "solar:emoji-funny-square-bold-duotone",
    },
  ],
  navSecondary: [
    {
      title: "Privacy",
      to: "/privacy",
      icon: "solar:eye-scan-bold-duotone",
    },
    {
      title: "Documentation",
      to: "https://docs.nullbox.email",
      external: true,
      icon: "carbon:document-multiple-01",
    },
    {
      title: "Source code",
      to: "https://github.com/nullbox-email",
      external: true,
      icon: "carbon:logo-github",
    },
    {
      title: "Github",
      to: "https://github.com/nullbox-email/support",
      external: true,
      icon: "carbon:logo-github",
    },
    {
      title: "Discord",
      to: "https://discord.gg/bRP9xfa7",
      external: true,
      icon: "carbon:logo-discord",
    },
  ],
};

const { mailboxes, ensureMailboxes } = useGlobalMailboxes();
await ensureMailboxes();
</script>

<template>
  <Sidebar v-bind="props">
    <SidebarHeader>
      <SidebarMenu>
        <SidebarMenuItem>
          <SidebarMenuButton size="lg" class="p-0" as-child>
            <nuxt-link to="/">
              <img
                src="/assets/logo.light.svg"
                alt="Nullbox Logo"
                class="block h-10 w-auto dark:hidden"
              />
              <img
                src="/assets/logo.dark.svg"
                alt="Nullbox Logo"
                class="hidden h-10 w-auto dark:block"
              />
            </nuxt-link>
          </SidebarMenuButton>
        </SidebarMenuItem>
      </SidebarMenu>
    </SidebarHeader>
    <SidebarContent>
      <nav-links :items="data.navMain" />
      <nav-mailboxes v-for="m in mailboxes" :key="m.id" v-bind="m" />
      <nav-links :items="data.navSecondary" class="mt-auto" />
    </SidebarContent>
    <SidebarFooter>
      <nav-version />
      <nav-user />
    </SidebarFooter>
  </Sidebar>
</template>

<i18n lang="yaml" scope="local">
en:
  title: Nullbox
  description: Less tracking. Same email.
</i18n>
