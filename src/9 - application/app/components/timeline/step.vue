<script setup lang="ts">
import type { HTMLAttributes } from "vue"
import { computed } from "vue"
import { cn } from "@/lib/utils"

type Breakpoint = "sm" | "md" | "lg" | "xl" | "2xl"

interface Props {
  number: string | number
  title: string
  isLast?: boolean
  /** Show left rail + line starting from this breakpoint. If omitted, always compact. */
  lineFrom?: Breakpoint
  class?: HTMLAttributes["class"]
}

const props = withDefaults(defineProps<Props>(), {
  isLast: false,
})

const bp = computed(() => {
  if (!props.lineFrom) return null
  const map: Record<Breakpoint, { compactVisible: string; railVisible: string }> = {
    sm: { compactVisible: "sm:hidden", railVisible: "sm:block" },
    md: { compactVisible: "md:hidden", railVisible: "md:block" },
    lg: { compactVisible: "lg:hidden", railVisible: "lg:block" },
    xl: { compactVisible: "xl:hidden", railVisible: "xl:block" },
    "2xl": { compactVisible: "2xl:hidden", railVisible: "2xl:block" },
  }
  return map[props.lineFrom]
})
</script>

<template>
  <li data-slot="timeline-step" :class="cn('relative', props.class)">
    <!-- COMPACT: number becomes part of title, content moves left -->
    <div :class="cn('space-y-1', bp?.compactVisible)">
      <div data-slot="timeline-step-title" class="flex items-baseline gap-3 font-semibold">
        <span
          class="flex h-7 w-7 shrink-0 items-center justify-center rounded-full border bg-background text-sm font-semibold"
        >
          {{ number }}
        </span>
        <span class="min-w-0">
          {{ title }}
        </span>
      </div>

      <div
        data-slot="timeline-step-body"
        class="text-muted-foreground [&>*]:text-foreground"
      >
        <slot name="body" />
      </div>
    </div>

    <!-- RAIL: existing layout with left line -->
    <div v-if="bp" :class="cn('hidden', bp.railVisible)">
      <div
        v-if="!props.isLast"
        data-slot="timeline-step-separator"
        class="absolute left-[13px] top-7 bottom-0 w-px bg-border"
      />

      <div
        data-slot="timeline-step-indicator"
        class="absolute left-0 mt-1 flex h-7 w-7 items-center justify-center rounded-full border bg-background text-sm font-semibold"
      >
        {{ number }}
      </div>

      <div data-slot="timeline-step-content" class="space-y-1 pl-12">
        <div data-slot="timeline-step-title" class="font-semibold">
          {{ title }}
        </div>

        <div
          data-slot="timeline-step-body"
          class="text-muted-foreground [&>*]:text-foreground"
        >
          <slot name="body" />
        </div>
      </div>
    </div>
  </li>
</template>
