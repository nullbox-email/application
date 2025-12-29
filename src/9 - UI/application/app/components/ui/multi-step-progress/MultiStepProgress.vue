<script setup lang="ts">
interface Step {
  text: string
  afterText?: string
  /** If true, this step shows the spinner */
  busy?: boolean
}

interface Props {
  steps: Step[]
}

const props = defineProps<Props>()

function isBusy(index: number) {
  return !!props.steps[index]?.busy
}

/**
 * A step is "completed" if any later step is busy.
 * (With your computed steps, this means earlier steps become completed as progress moves forward.)
 */
function isCompleted(index: number) {
  for (let i = index + 1; i < props.steps.length; i++) {
    if (props.steps[i]?.busy) return true
  }
  return false
}

function isNotStarted(index: number) {
  return !isBusy(index) && !isCompleted(index)
}

function textClass(index: number) {
  return isBusy(index) ? "text-foreground" : "text-muted-foreground"
}

function iconClass(index: number) {
  return isBusy(index) ? "text-primary" : "text-muted-foreground"
}
</script>

<template>
  <div class="space-y-3">
    <div v-for="(step, index) in props.steps" :key="index" class="flex items-start gap-3">
      <!-- Icon -->
      <div class="mt-0.5">
        <!-- completed -->
        <svg
          v-if="isCompleted(index)"
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 24 24"
          fill="currentColor"
          class="size-5"
          :class="iconClass(index)"
        >
          <path
            fill-rule="evenodd"
            d="M2.25 12c0-5.385 4.365-9.75 9.75-9.75s9.75 4.365 9.75 9.75-4.365 9.75-9.75 9.75S2.25 17.385 2.25 12Zm13.36-1.814a.75.75 0 1 0-1.22-.872l-3.236 4.53L9.53 12.22a.75.75 0 0 0-1.06 1.06l2.25 2.25a.75.75 0 0 0 1.14-.094l3.75-5.25Z"
            clip-rule="evenodd"
          />
        </svg>

        <!-- busy -->
        <svg
          v-else-if="isBusy(index)"
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 24 24"
          fill="currentColor"
          class="size-5 animate-spin"
          :class="iconClass(index)"
        >
          <path
            fill-rule="evenodd"
            d="M4.755 10.059a7.5 7.5 0 0 1 12.548-3.364l1.903 1.903h-3.183a.75.75 0 1 0 0 1.5h4.992a.75.75 0 0 0 .75-.75V4.356a.75.75 0 0 0-1.5 0v3.18l-1.9-1.9A9 9 0 0 0 3.306 9.67a.75.75 0 1 0 1.45.388Zm15.408 3.352a.75.75 0 0 0-.919.53 7.5 7.5 0 0 1-12.548 3.364l-1.902-1.903h3.183a.75.75 0 0 0 0-1.5H2.984a.75.75 0 0 0-.75.75v4.992a.75.75 0 0 0 1.5 0v-3.18l1.9 1.9a9 9 0 0 0 15.059-4.035.75.75 0 0 0-.53-.918Z"
            clip-rule="evenodd"
          />
        </svg>

        <!-- not started -->
        <svg
          v-else
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
          stroke-width="1.5"
          stroke="currentColor"
          class="size-5"
          :class="iconClass(index)"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            d="M9 12.75 11.25 15 15 9.75M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z"
          />
        </svg>
      </div>

      <!-- Text -->
      <div class="flex min-w-0 flex-col">
        <span :class="textClass(index)">
          {{ step.text }}
        </span>

        <span v-if="step.afterText && isCompleted(index)" class="mt-1 text-muted-foreground">
          {{ step.afterText }}
        </span>
      </div>
    </div>
  </div>
</template>
