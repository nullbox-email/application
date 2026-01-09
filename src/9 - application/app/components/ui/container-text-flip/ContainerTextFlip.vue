<template>
  <Motion
    :key="words[currentWordIndex]"
    as="span"
    :layout-id="`words-here-${id}`"
    :animate="{ width }"
    :transition="{ duration: props.animationDuration / 2000 }"
    :class="cn(props.class)"
  >
    <Motion
      ref="textRef"
      as="div"
      :transition="{
        duration: animationDuration / 1000,
        ease: 'easeInOut',
      }"
      :class="cn('inline-block', props.textClass)"
      :layout-id="`word-div-${words[currentWordIndex]}-${id}`"
    >
      <Motion as="div" class="inline-block">
        <Motion
          v-for="(letter, index) in words[currentWordIndex]"
          :key="index"
          as="span"
          :initial="{
            opacity: 0,
            filter: 'blur(10px)',
          }"
          :animate="{
            opacity: 1,
            filter: 'blur(0px)',
          }"
          :transition="{
            delay: index * 0.02,
          }"
        >
          {{ letter }}
        </Motion>
      </Motion>
    </Motion>
  </Motion>
</template>

<script lang="ts" setup>
import { cn } from "@/lib/utils";
import { Motion } from "motion-v";
import { useIntervalFn } from "@vueuse/core";

const props = withDefaults(
  defineProps<{
    words?: string[];
    interval?: number;
    animationDuration?: number;
    class?: string;
    textClass?: string;
  }>(),
  {
    words: () => ["better", "modern", "beautiful", "awesome"],
    interval: 3000,
    animationDuration: 700,
  }
);

const id = useId();

const currentWordIndex = ref(0);
const textRef = useTemplateRef<HTMLDivElement>("textRef");

const width = computed(() => {
  if (textRef.value) {
    return textRef.value.scrollWidth + 10;
  }
  return 100;
});

useIntervalFn(() => {
  currentWordIndex.value = (currentWordIndex.value + 1) % props.words.length;
}, props.interval);
</script>

<style scoped></style>
