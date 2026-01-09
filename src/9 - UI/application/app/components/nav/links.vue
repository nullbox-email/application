<script setup lang="ts">
interface NavItem {
  title: string;
  to: string;
  external?: boolean;
  icon?: string;
  icon_class?: string;
}

defineProps<{
  items: NavItem[];
}>();
</script>

<template>
  <SidebarGroup>
    <SidebarGroupContent class="flex flex-col gap-2">
      <SidebarMenu>
        <SidebarMenuItem v-for="item in items" :key="item.title">
          <SidebarMenuButton :tooltip="item.title" as-child>
            <nuxt-link
              :to="item.to"
              :external="item.external ?? false"
              :target="item.external ? '_blank' : null"
              class="inline-flex items-center gap-2"
            >
              <icon
                v-if="item.icon"
                :name="item.icon"
                class="text-xl"
                :class="item.icon_class"
              />
              <span class="leading-none">{{ item.title }}</span>
            </nuxt-link>
          </SidebarMenuButton>
        </SidebarMenuItem>
      </SidebarMenu>
    </SidebarGroupContent>
  </SidebarGroup>
</template>
