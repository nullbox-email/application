<template>
  <div ref="rootRef" class="relative size-full overflow-hidden" v-bind="$attrs">
    <canvas ref="canvasRef" class="absolute inset-0 block size-full" />
    <slot />
  </div>
</template>

<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from "vue";

type RGB = [number, number, number];

interface Particle {
  x: number;
  y: number;
  vy: number;
  r: number; // logical size scalar
  rot: number;
  vr: number; // rotation velocity
  a: number; // alpha
  drift: number; // small sideways drift
}

interface Props {
  /** Number of particles */
  count?: number;

  /** Falling speed multiplier */
  speed?: number;

  /** Particle size multiplier (applied to r) */
  spriteScale?: number;

  /** Base RGB for particles */
  particleRGB?: RGB;

  /** Alpha range for particles */
  alphaMin?: number;
  alphaMax?: number;

  /** Canvas opacity (overall) */
  opacity?: number;

  /** Stroke/fill mode */
  mode?: "fill" | "stroke";

  /** Stroke width if mode=stroke (in SVG units, scaled with sprite) */
  strokeWidth?: number;

  /**
   * SVG paths to draw as the particle “sprite”.
   * Default: the 24x24 mail icon (two paths) you already used.
   */
  svgPaths?: string[];

  /**
   * If true, particles wrap from bottom to top; otherwise they respawn randomly.
   */
  wrap?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  count: 110,
  speed: 1,
  spriteScale: 1.25,
  particleRGB: () => [255, 255, 255],
  alphaMin: 0.15,
  alphaMax: 0.9,
  opacity: 0.18,
  mode: "fill",
  strokeWidth: 1.25,
  wrap: false,
  svgPaths: () => [
    "M1.5 8.67v8.58a3 3 0 0 0 3 3h15a3 3 0 0 0 3-3V8.67l-8.928 5.493a3 3 0 0 1-3.144 0z",
    "M22.5 6.908V6.75a3 3 0 0 0-3-3h-15a3 3 0 0 0-3 3v.158l9.714 5.978a1.5 1.5 0 0 0 1.572 0z",
  ],
});

const rootRef = ref<HTMLElement | null>(null);
const canvasRef = ref<HTMLCanvasElement | null>(null);

const rafRef = ref<number>(0);
let ro: ResizeObserver | null = null;

const dpr = ref(1);
const size = ref({ w: 0, h: 0 });

const sprite = computed(() => props.svgPaths.map((d) => new Path2D(d)));

const particles = ref<Particle[]>([]);

function rand(min: number, max: number) {
  return min + Math.random() * (max - min);
}

function resizeToContainer() {
  const root = rootRef.value;
  const canvas = canvasRef.value;
  if (!root || !canvas) return;

  const rect = root.getBoundingClientRect();
  size.value = { w: Math.max(1, rect.width), h: Math.max(1, rect.height) };
  dpr.value = window.devicePixelRatio || 1;

  canvas.width = Math.floor(size.value.w * dpr.value);
  canvas.height = Math.floor(size.value.h * dpr.value);
}

function makeParticle(initial = false): Particle {
  const { w, h } = size.value;

  // Slightly wider spawn area for natural spread
  const x = rand(-w * 0.05, w * 1.05);

  // If initial, distribute throughout height; else spawn above top
  const y = initial ? rand(0, h) : rand(-h * 0.25, -12);

  const r = rand(0.6, 1.9);
  const vy = rand(0.35, 1.1) * props.speed;

  return {
    x,
    y,
    vy,
    r,
    rot: rand(0, Math.PI * 2),
    vr: rand(-0.02, 0.02),
    a: rand(props.alphaMin, props.alphaMax),
    drift: rand(-0.15, 0.15),
  };
}

function initParticles() {
  particles.value = Array.from({ length: props.count }, () => makeParticle(true));
}

function step() {
  const canvas = canvasRef.value;
  if (!canvas) return;

  const ctx = canvas.getContext("2d");
  if (!ctx) return;

  const { w, h } = size.value;

  ctx.clearRect(0, 0, canvas.width, canvas.height);
  ctx.save();
  ctx.scale(dpr.value, dpr.value);

  // Global opacity control
  ctx.globalAlpha = props.opacity;

  // Draw
  const paths = sprite.value;
  const iconBox = 24;
  const iconHalf = iconBox / 2;

  for (let i = 0; i < particles.value.length; i++) {
    const p = particles.value[i];

    // update
    p.y += p.vy;
    p.x += p.drift;
    p.rot += p.vr;

    // respawn
    if (p.y > h + 24) {
      particles.value[i] = props.wrap
        ? { ...p, y: -24 }
        : makeParticle(false);
      continue;
    }

    // per-particle alpha & color
    ctx.save();
    ctx.globalAlpha = props.opacity * p.a;

    const [r, g, b] = props.particleRGB;
    ctx.fillStyle = `rgba(${r}, ${g}, ${b}, 1)`;
    ctx.strokeStyle = `rgba(${r}, ${g}, ${b}, 1)`;

    // place + rotate + scale
    ctx.translate(p.x, p.y);
    ctx.rotate(p.rot);

    const s = p.r * props.spriteScale;
    ctx.scale(s, s);

    // center 24x24 sprite around origin
    ctx.translate(-iconHalf, -iconHalf);

    if (props.mode === "stroke") {
      ctx.lineWidth = props.strokeWidth;
      for (const path of paths) ctx.stroke(path);
    } else {
      for (const path of paths) ctx.fill(path);
    }

    ctx.restore();
  }

  ctx.restore();
  rafRef.value = requestAnimationFrame(step);
}

onMounted(() => {
  resizeToContainer();
  initParticles();
  rafRef.value = requestAnimationFrame(step);

  // ResizeObserver so it works inside grid columns / responsive layouts
  ro = new ResizeObserver(() => {
    resizeToContainer();
    initParticles();
  });
  if (rootRef.value) ro.observe(rootRef.value);
});

onBeforeUnmount(() => {
  if (ro && rootRef.value) ro.unobserve(rootRef.value);
  ro = null;
  cancelAnimationFrame(rafRef.value);
});
</script>
