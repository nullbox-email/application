<script setup lang="ts">
interface Alias {
  id: string;
  name: string;
  localPart: string;
  isEnabled: boolean;
}

defineProps<{
  id: string;
  name: string;
  routingKey: string;
  domain: string;
  aliases: Alias[];
}>();
</script>

<template>
  <SidebarGroup class="group-data-[collapsible=icon]:hidden">
    <SidebarGroupLabel>{{ name }}</SidebarGroupLabel>
    <SidebarMenu>
      <SidebarMenuItem>
        <SidebarMenuButton as-child>
          <nuxt-link
            :to="`/mailboxes/${routingKey}.${domain}/getting-started`"
            class="flex items-center justify-between gap-2"
          >
            <span>Get started</span>
            <icon name="solar:planet-2-bold-duotone" class="text-xl" />
          </nuxt-link>
        </SidebarMenuButton>
      </SidebarMenuItem>
      <SidebarMenuItem v-if="aliases && aliases.length > 0">
        <SidebarMenuButton as-child>
          <nuxt-link
            :to="`/mailboxes/${routingKey}.${domain}`"
            class="flex items-center justify-between gap-2"
          >
            <span>Dashboard</span>
          </nuxt-link>
        </SidebarMenuButton>
      </SidebarMenuItem>
    </SidebarMenu>
    <SidebarGroupLabel>Aliases</SidebarGroupLabel>
    <SidebarMenu>
      <SidebarMenuItem>
        <SidebarMenuButton as-child>
          <nuxt-link
            :to="`/mailboxes/${routingKey}.${domain}/alias/create`"
            class="flex items-center justify-between gap-2"
          >
            <span>Create alias</span>
            <icon name="solar:user-plus-bold-duotone" class="text-xl" />
          </nuxt-link>
        </SidebarMenuButton>
      </SidebarMenuItem>
      <SidebarMenuItem v-for="alias in aliases" :key="alias.name">
        <SidebarMenuButton as-child>
          <nuxt-link
            :to="`/mailboxes/${routingKey}.${domain}/${alias.localPart}`"
            class="flex items-center gap-2"
          >
            <span>{{ alias.name }}</span>
          </nuxt-link>
        </SidebarMenuButton>
      </SidebarMenuItem>
    </SidebarMenu>
  </SidebarGroup>
</template>
