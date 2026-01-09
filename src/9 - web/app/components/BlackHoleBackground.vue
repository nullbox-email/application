<template>
  <div
    data-slot="black-hole-background"
    :class="[
      'relative size-full overflow-hidden',
      `before:absolute before:left-1/2 before:top-1/2 before:block before:size-[140%] before:content-[''] before:[background:radial-gradient(ellipse_at_50%_55%,transparent_10%,white_50%)] before:[transform:translate3d(-50%,-50%,0)] dark:before:[background:radial-gradient(ellipse_at_50%_55%,transparent_10%,black_50%)]`,
      `after:absolute after:left-1/2 after:top-1/2 after:z-[5] after:block after:size-full after:mix-blend-overlay after:content-[''] after:[background:radial-gradient(ellipse_at_50%_75%,#a900ff_20%,transparent_75%)] after:[transform:translate3d(-50%,-50%,0)]`,
    ]"
    v-bind="props"
  >
    <slot></slot>
    <canvas
      ref="canvasRef"
      class="absolute inset-0 block size-full opacity-10 dark:opacity-20"
    />
    <motion.div
      :class="[
        'absolute left-1/2 top-[-71.5%] z-[3] h-[140%] w-[30%] rounded-b-full opacity-75 mix-blend-plus-darker blur-3xl [background-position:0%_100%] [background-size:100%_200%] [transform:translate3d(-50%,0,0)] dark:mix-blend-plus-lighter',
        '[background:linear-gradient(20deg,#00f8f1,#ffbd1e40_16.5%,#fe848f_33%,#fe848f40_49.5%,#00f8f1_66%,#00f8f180_85.5%,#ffbd1e_100%)_0_100%_/_100%_200%] dark:[background:linear-gradient(20deg,#00f8f1,#ffbd1e20_16.5%,#fe848f_33%,#fe848f20_49.5%,#00f8f1_66%,#00f8f160_85.5%,#ffbd1e_100%)_0_100%_/_100%_200%]',
      ]"
      :animate="{ backgroundPosition: '0% 300%' }"
      :transition="{ duration: 5, ease: 'linear', repeat: Infinity }"
    />
    <div
      class="absolute left-0 top-0 z-[7] size-full opacity-50 mix-blend-overlay dark:[background:repeating-linear-gradient(transparent,transparent_1px,white_1px,white_2px)]"
    />
  </div>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { motion } from "motion-v";

interface Disc {
  p: number;
  x: number;
  y: number;
  w: number;
  h: number;
}

interface Point {
  x: number;
  y: number;
}

interface Particle {
  x: number;
  y: number;
  vy: number;

  x0: number;
  xMid: number; // new: funnel entry point
  x1: number;

  phase: number;
  p: number;
  r: number;
  c: string;
}

interface Clip {
  disc?: Disc;
  i?: number;
  path?: Path2D;
}

interface State {
  discs: Disc[];
  lines: Point[][];
  particles: Particle[];
  clip: Clip;
  startDisc: Disc;
  endDisc: Disc;
  rect: { width: number; height: number };
  render: { width: number; height: number; dpi: number };
  particleArea: {
    sw?: number;
    ew?: number;
    h?: number;
    sx?: number;
    ex?: number;
  };
  linesCanvas?: HTMLCanvasElement;
}

interface Props {
  strokeColor?: string;
  numberOfLines?: number;
  numberOfDiscs?: number;
  particleRGBColor?: [number, number, number];
  /** Multiplier from particle.r -> icon size */
  particleSpriteScale?: number;
  class?: string;
}

const props = withDefaults(defineProps<Props>(), {
  strokeColor: "#737373",
  numberOfLines: 50,
  numberOfDiscs: 50,
  particleRGBColor: () => [255, 255, 255],
  particleSpriteScale: 1.2,
});

const canvasRef = ref<HTMLCanvasElement | null>(null);
const animationFrameIdRef = ref<number>(0);

const stateRef = ref<State>({
  discs: [],
  lines: [],
  particles: [],
  clip: {},
  startDisc: { p: 0, x: 0, y: 0, w: 0, h: 0 },
  endDisc: { p: 0, x: 0, y: 0, w: 0, h: 0 },
  rect: { width: 0, height: 0 },
  render: { width: 0, height: 0, dpi: 1 },
  particleArea: {},
});

/**
 * Particle sprite: mail icon (24x24) using two SVG paths.
 * We draw it with Path2D so fillStyle can stay per-particle (particle.c).
 */
const particleSprite = computed(() => {
  const p1 = new Path2D(
    "M1.5 8.67v8.58a3 3 0 0 0 3 3h15a3 3 0 0 0 3-3V8.67l-8.928 5.493a3 3 0 0 1-3.144 0z"
  );
  const p2 = new Path2D(
    "M22.5 6.908V6.75a3 3 0 0 0-3-3h-15a3 3 0 0 0-3 3v.158l9.714 5.978a1.5 1.5 0 0 0 1.572 0z"
  );
  return [p1, p2] as const;
});

function linear(p: number) {
  return p;
}

function easeInExpo(p: number) {
  return p === 0 ? 0 : Math.pow(2, 10 * (p - 1));
}

function clamp(n: number, min: number, max: number) {
  return Math.max(min, Math.min(max, n));
}

// Triangular distribution in [0,1] (dense in the middle)
function tri01() {
  return (Math.random() + Math.random()) / 2;
}

function tweenValue(
  start: number,
  end: number,
  p: number,
  ease: "inExpo" | null = null
) {
  const delta = end - start;
  const easeFn = ease === "inExpo" ? easeInExpo : linear;
  return start + delta * easeFn(p);
}

function tweenDisc(disc: Disc) {
  const { startDisc, endDisc } = stateRef.value;
  disc.x = tweenValue(startDisc.x, endDisc.x, disc.p);
  disc.y = tweenValue(startDisc.y, endDisc.y, disc.p, "inExpo");
  disc.w = tweenValue(startDisc.w, endDisc.w, disc.p);
  disc.h = tweenValue(startDisc.h, endDisc.h, disc.p);
}

function setSize() {
  const canvas = canvasRef.value;
  if (!canvas) return;
  const rect = canvas.getBoundingClientRect();
  stateRef.value.rect = { width: rect.width, height: rect.height };
  stateRef.value.render = {
    width: rect.width,
    height: rect.height,
    dpi: window.devicePixelRatio || 1,
  };
  canvas.width = stateRef.value.render.width * stateRef.value.render.dpi;
  canvas.height = stateRef.value.render.height * stateRef.value.render.dpi;
}

function setDiscs() {
  const { width, height } = stateRef.value.rect;
  if (width <= 0 || height <= 0) return;

  stateRef.value.discs = [];
  stateRef.value.startDisc = {
    p: 0,
    x: width * 0.5,
    y: height * 0.45,
    w: width * 0.75,
    h: height * 0.7,
  };
  stateRef.value.endDisc = {
    p: 0,
    x: width * 0.5,
    y: height * 0.95,
    w: 0,
    h: 0,
  };

  let prevBottom = height;
  stateRef.value.clip = {};

  for (let i = 0; i < props.numberOfDiscs; i++) {
    const p = i / props.numberOfDiscs;
    const disc = { p, x: 0, y: 0, w: 0, h: 0 };
    tweenDisc(disc);
    const bottom = disc.y + disc.h;
    if (bottom <= prevBottom) {
      stateRef.value.clip = { disc: { ...disc }, i };
    }
    prevBottom = bottom;
    stateRef.value.discs.push(disc);
  }

  if (stateRef.value.clip.disc) {
    const clipPath = new Path2D();
    const disc = stateRef.value.clip.disc;
    clipPath.ellipse(disc.x, disc.y, disc.w, disc.h, 0, 0, Math.PI * 2);
    const { width } = stateRef.value.rect;
    // allow particles to appear across the full width above the hole
    clipPath.rect(0, 0, width, disc.y);
    stateRef.value.clip.path = clipPath;
  }
}

function setLines() {
  const { width, height } = stateRef.value.rect;
  if (width <= 0 || height <= 0) return;

  stateRef.value.lines = [];
  const linesAngle = (Math.PI * 2) / props.numberOfLines;
  for (let i = 0; i < props.numberOfLines; i++) {
    stateRef.value.lines.push([]);
  }

  stateRef.value.discs.forEach((disc: Disc) => {
    for (let i = 0; i < props.numberOfLines; i++) {
      const angle = i * linesAngle;
      const p = {
        x: disc.x + Math.cos(angle) * disc.w,
        y: disc.y + Math.sin(angle) * disc.h,
      };
      stateRef.value.lines[i].push(p);
    }
  });

  const offCanvas = document.createElement("canvas");
  offCanvas.width = Math.max(1, width);
  offCanvas.height = Math.max(1, height);

  const ctx = offCanvas.getContext("2d");
  if (!ctx || !stateRef.value.clip.path) {
    stateRef.value.linesCanvas = undefined;
    return;
  }

  ctx.clearRect(0, 0, offCanvas.width, offCanvas.height);

  stateRef.value.lines.forEach((line: Point[]) => {
    ctx.save();
    let lineIsIn = false;
    line.forEach((p1: Point, j: number) => {
      if (j === 0) return;
      const p0 = line[j - 1];
      if (
        !lineIsIn &&
        (ctx.isPointInPath(stateRef.value.clip.path!, p1.x, p1.y) ||
          ctx.isPointInStroke(stateRef.value.clip.path!, p1.x, p1.y))
      ) {
        lineIsIn = true;
      } else if (lineIsIn) {
        ctx.clip(stateRef.value.clip.path!);
      }
      ctx.beginPath();
      ctx.moveTo(p0.x, p0.y);
      ctx.lineTo(p1.x, p1.y);
      ctx.strokeStyle = props.strokeColor;
      ctx.lineWidth = 2;
      ctx.stroke();
      ctx.closePath();
    });
    ctx.restore();
  });

  stateRef.value.linesCanvas = offCanvas;
}

function setParticles() {
  const { width, height } = stateRef.value.rect;
  stateRef.value.particles = [];

  const disc = stateRef.value.clip.disc;
  if (!disc || width <= 0 || height <= 0) return;

  // Particles travel the full height, but "fall into" the hole near the disc center.
  // You can tune endYFactor to make them disappear earlier/later.
  const endYFactor = 0.92;

  stateRef.value.particleArea = {
    h: height * endYFactor,
  };

  // Scale particle count with available width (smaller screens = fewer particles)
  const w = stateRef.value.rect.width;

  // 320px -> ~70, 768px -> ~140, 1280px -> ~200, 1920px -> ~260
  const totalParticles = Math.round(clamp((w / 1280) * 200, 70, 260));

  for (let i = 0; i < totalParticles; i++) {
    stateRef.value.particles.push(initParticle(true));
  }
}

function initParticle(start: boolean = false): Particle {
  const { width, height } = stateRef.value.rect;
  const h = stateRef.value.particleArea.h || 0;

  const disc = stateRef.value.clip.disc;
  const cx = disc ? disc.x : width / 2;

  // Spawn across (almost) full width
  const spread = 0.98;
  const usable = width * spread;
  const left = (width - usable) / 2;
  const x0 = left + tri01() * usable;

  // Compute a "capture" height where particles should already be inside the funnel.
  // Higher value = they start funneling sooner.
  const captureY = height * 0.35;

  // Approx funnel half-width at capture height (wide near top, narrows down).
  // Tie to the outer disc size so it matches your drawn funnel.
  const outer = stateRef.value.startDisc;
  const topHalf = outer.w; // startDisc.w is already the radius used in line points
  const midHalf = disc ? disc.w * 0.9 : topHalf * 0.4;

  // Map captureY into 0..1 between outerDisc.y (top) and disc.y (hole area)
  const y0 = outer.y;
  const y1 = disc ? disc.y : height * 0.8;
  const tt = clamp((captureY - y0) / Math.max(1, y1 - y0), 0, 1);

  // Linear funnel width falloff (can swap to ease for different feel)
  const funnelHalfAtCapture = topHalf + (midHalf - topHalf) * tt;

  // xMid is the point on the funnel rim that corresponds to where you spawned
  // (keeps symmetry: left spawns enter left side of funnel, etc.)
  const side = x0 < cx ? -1 : 1;
  // xMid must be closer to the center than x0 (otherwise you get outward motion)
  const d0 = x0 - cx;
  const d0abs = Math.abs(d0);

  // How much closer to the center we move by the time we reach the "capture" phase.
  // Smaller = stronger inward pull earlier.
  const captureShrink = 0.35; // try 0.25 for even stronger inward flow

  // Compute a mid-distance that is always < d0abs (so it always moves inward).
  // Add a small minimum so it is still visible for near-center spawns.
  let dMid = d0abs * captureShrink;
  dMid = Math.max(6, Math.min(dMid, d0abs - 1)); // ensure strictly inward

  // Preserve left/right side
  const xMid = cx + Math.sign(d0 || 1) * dMid;

  // Singularity target: very tight band
  const targetHalfWidth = disc ? Math.max(3, disc.w * 0.06) : 12;
  const x1 = cx + (Math.random() * 2 - 1) * targetHalfWidth;

  const y = start ? h * Math.random() : -20 - Math.random() * 60;

  const r = 0.5 + Math.random() * 1.5;
  const vy = 0.6 + Math.random() * 1.4;

  return {
    x: x0,
    y,
    vy,

    x0,
    xMid,
    x1,

    phase: Math.random() * Math.PI * 2,
    p: 0,
    r,
    c: `rgba(${props.particleRGBColor[0]}, ${props.particleRGBColor[1]}, ${
      props.particleRGBColor[2]
    }, ${0.15 + Math.random() * 0.65})`,
  };
}

function moveParticles() {
  const h = stateRef.value.particleArea.h || 1;
  const disc = stateRef.value.clip.disc;

  // phase split: first phase quickly gets them into the funnel
  const tCapture = 0.5; // lower = later capture, higher = earlier capture

  const easeOut = (t: number) => 1 - Math.pow(1 - clamp(t, 0, 1), 3);
  const easeInHard = (t: number) => Math.pow(clamp(t, 0, 1), 6);

  const swirlAmpMax = disc ? Math.max(4, disc.w * 0.02) : 6;
  const swirlFreq = 7;

  stateRef.value.particles.forEach((particle, idx) => {
    particle.y += particle.vy;

    const t = particle.y / h;
    particle.p = t;

    let x: number;

    if (t < tCapture) {
      // x0 -> xMid relatively quickly
      const p1 = easeOut(t / tCapture);
      x = particle.x0 + (particle.xMid - particle.x0) * p1;
    } else {
      // xMid -> x1 very tight at the bottom
      const p2 = easeInHard((t - tCapture) / (1 - tCapture));
      x = particle.xMid + (particle.x1 - particle.xMid) * p2;
    }

    // swirl, but fade it strongly near the end so the singularity stays tight
    const swirlFade = Math.pow(1 - clamp(t, 0, 1), 4);
    const swirl =
      Math.sin(particle.phase + t * Math.PI * 2 * swirlFreq) *
      swirlAmpMax *
      swirlFade;

    particle.x = x + swirl;

    if (particle.y > h) {
      stateRef.value.particles[idx] = initParticle(false);
    }
  });
}

function drawDiscs(ctx: CanvasRenderingContext2D) {
  ctx.strokeStyle = props.strokeColor;
  ctx.lineWidth = 2;

  const outerDisc = stateRef.value.startDisc;
  ctx.beginPath();
  ctx.ellipse(
    outerDisc.x,
    outerDisc.y,
    outerDisc.w,
    outerDisc.h,
    0,
    0,
    Math.PI * 2
  );
  ctx.stroke();
  ctx.closePath();

  stateRef.value.discs.forEach((disc: Disc, i: number) => {
    if (i % 5 !== 0) return;
    if (disc.w < (stateRef.value.clip.disc?.w || 0) - 5) {
      ctx.save();
      ctx.clip(stateRef.value.clip.path!);
    }
    ctx.beginPath();
    ctx.ellipse(disc.x, disc.y, disc.w, disc.h, 0, 0, Math.PI * 2);
    ctx.stroke();
    ctx.closePath();
    if (disc.w < (stateRef.value.clip.disc?.w || 0) - 5) {
      ctx.restore();
    }
  });
}

function drawLines(ctx: CanvasRenderingContext2D) {
  if (
    stateRef.value.linesCanvas &&
    stateRef.value.linesCanvas.width > 0 &&
    stateRef.value.linesCanvas.height > 0
  ) {
    ctx.drawImage(stateRef.value.linesCanvas, 0, 0);
  }
}

function drawParticles(ctx: CanvasRenderingContext2D) {
  ctx.save();
  ctx.clip(stateRef.value.clip.path!);

  const [p1, p2] = particleSprite.value;

  // Original icon viewBox is 24x24; center it around (0,0)
  const iconBox = 24;
  const iconHalf = iconBox / 2;

  stateRef.value.particles.forEach((particle: Particle) => {
    ctx.save();

    // Position at particle location
    ctx.translate(particle.x, particle.y);

    // Scale: particle.r is small; scale up for visibility
    const s = particle.r * props.particleSpriteScale;
    ctx.scale(s, s);

    // Center the icon on the particle point
    ctx.translate(-iconHalf, -iconHalf);

    ctx.fillStyle = particle.c;
    ctx.fill(p1);
    ctx.fill(p2);

    ctx.restore();
  });

  ctx.restore();
}

function moveDiscs() {
  stateRef.value.discs.forEach((disc: Disc) => {
    disc.p = (disc.p + 0.001) % 1;
    tweenDisc(disc);
  });
}

function tick() {
  const canvas = canvasRef.value;
  if (!canvas) return;

  const ctx = canvas.getContext("2d");
  if (!ctx) return;

  ctx.clearRect(0, 0, canvas.width, canvas.height);

  ctx.save();
  ctx.scale(stateRef.value.render.dpi, stateRef.value.render.dpi);

  moveDiscs();
  moveParticles();
  drawDiscs(ctx);
  drawLines(ctx);
  drawParticles(ctx);

  ctx.restore();
  animationFrameIdRef.value = requestAnimationFrame(tick);
}

function init() {
  setSize();
  setDiscs();
  setLines();
  setParticles();
}

function handleResize() {
  setSize();
  setDiscs();
  setLines();
  setParticles();
}

onMounted(() => {
  nextTick(() => {
    setSize();
    init();
    tick();
    window.addEventListener("resize", handleResize);
  });
});

onBeforeUnmount(() => {
  window.removeEventListener("resize", handleResize);
  cancelAnimationFrame(animationFrameIdRef.value);
});
</script>
