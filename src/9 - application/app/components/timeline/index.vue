<script setup lang="ts">
import type { HTMLAttributes } from "vue"
import { cn } from "@/lib/utils"

interface Props {
  class?: HTMLAttributes["class"]
}

const props = defineProps<Props>()
</script>

<template>
  <ol
    data-slot="timeline"
    :class="cn('relative', props.class)"
  >
    <slot />
  </ol>
</template>
