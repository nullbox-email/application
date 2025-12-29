<script setup lang="ts">
import { computed } from "vue";
import type { ChartConfig } from "@/components/ui/chart";

import { VisArea, VisAxis, VisLine, VisXYContainer } from "@unovis/vue";
import {
  ChartContainer,
  ChartCrosshair,
  ChartLegendContent,
  ChartTooltip,
  ChartTooltipContent,
  componentToString,
} from "@/components/ui/chart";

export type SeriesSpec = {
  key: string;
  label: string;
  color: string;
  fill?: {
    id: string;
    fromOpacity?: number;
    toOpacity?: number;
  };
};

export type TimePoint = {
  date?: Date | string | number;
  timestamp?: Date | string | number;
  [seriesKey: string]: unknown;
};

export type TimeGrain = "Hourly" | "Daily";

const props = withDefaults(
  defineProps<{
    data: TimePoint[];
    series: SeriesSpec[];
    /** Field name to use for the x value. Defaults to "timestamp". */
    xKey?: "timestamp" | "date" | (string & {});
    /** How to format timestamps on axis/tooltip. */
    grain?: TimeGrain;
    yDomain?: [number, number];
    stacked?: boolean;
    height?: number;
    /** Desired number of x ticks (used to downsample real data ticks). */
    xTicks?: number;
  }>(),
  {
    stacked: true,
    height: 250,
    xKey: "timestamp",
    grain: "Hourly",
    xTicks: undefined,
  }
);

function toDate(v: Date | string | number | undefined): Date {
  if (v instanceof Date) return v;
  return new Date(v as any);
}

function toNumber(v: unknown): number {
  const n = typeof v === "number" ? v : Number(v);
  return Number.isFinite(n) ? n : 0;
}

// Build ChartConfig dynamically from series spec
const chartConfig = computed(() => {
  const cfg: Record<string, { label: string; color: string }> = {};
  for (const s of props.series) cfg[s.key] = { label: s.label, color: s.color };
  return cfg satisfies ChartConfig;
});

// Normalize data: ensure x is Date, series values are numbers, and sort ascending by time
type NormalizedPoint = { date: Date } & Record<string, number>;
const normalized = computed<NormalizedPoint[]>(() => {
  const mapped = props.data.map((p) => {
    const rawX =
      (p as any)?.[props.xKey] ?? (p as any)?.timestamp ?? (p as any)?.date;

    const out: any = { date: toDate(rawX) };
    for (const s of props.series) out[s.key] = toNumber((p as any)[s.key]);
    return out as NormalizedPoint;
  });

  mapped.sort((a, b) => +a.date - +b.date);
  return mapped;
});

// Helpers for stacked rendering
const stackedYFns = computed(() => {
  const keys = props.series.map((s) => s.key);

  const area = keys.map((k) => (d: NormalizedPoint) => d[k]);

  const line = keys.map((_, idx) => {
    return (d: NormalizedPoint) => {
      let sum = 0;
      for (let i = 0; i <= idx; i++) sum += d[keys[i]];
      return sum;
    };
  });

  return { area, line, keys };
});

// yDomain: use provided or derive from max (stacked max if stacked)
const derivedYDomain = computed<[number, number]>(() => {
  if (props.yDomain) return props.yDomain;

  const keys = props.series.map((s) => s.key);
  let maxY = 0;

  for (const d of normalized.value) {
    if (props.stacked) {
      let sum = 0;
      for (const k of keys) sum += d[k] ?? 0;
      if (sum > maxY) maxY = sum;
    } else {
      for (const k of keys) if ((d[k] ?? 0) > maxY) maxY = d[k] ?? 0;
    }
  }

  const headroom = Math.max(1, Math.ceil(maxY * 0.1));
  return [0, maxY + headroom];
});

// Optional SVG defs for gradient fills (only if any series has fill)
const svgDefs = computed(() => {
  const defs = props.series
    .filter((s) => !!s.fill)
    .map((s) => {
      const id = s.fill!.id;
      const fromOpacity = s.fill!.fromOpacity ?? 0.8;
      const toOpacity = s.fill!.toOpacity ?? 0.1;

      return `
        <linearGradient id="${id}" x1="0" y1="0" x2="0" y2="1">
          <stop offset="5%" stop-color="${s.color}" stop-opacity="${fromOpacity}" />
          <stop offset="95%" stop-color="${s.color}" stop-opacity="${toOpacity}" />
        </linearGradient>
      `;
    })
    .join("\n");

  return defs || undefined;
});

const areaColors = computed(() =>
  props.series.map((s) => (s.fill ? `url(#${s.fill.id})` : s.color))
);
const lineColors = computed(() => props.series.map((s) => s.color));

// Formatting
function formatForGrain(ms: number) {
  const d = new Date(ms);

  if (props.grain === "Hourly") {
    const diffMs = Date.now() - d.getTime();
    const diffHours = Math.max(0, Math.floor(diffMs / 3_600_000));
    return diffHours === 0 ? "now" : `${diffHours}h ago`;
  }

  const diffMs = Date.now() - d.getTime();
  const diffDays = Math.max(0, Math.floor(diffMs / 86_400_000));
  return diffDays > 0 ? `${diffDays}d ago` : "today";
}

// Use the actual data points as tick positions (downsampled) to avoid “made up” times
const xTickValues = computed<number[]>(() => {
  const dates = normalized.value.map((d) => +d.date);
  if (dates.length === 0) return [];
  if (dates.length === 1) return dates;

  const desired = props.xTicks ?? (props.grain === "Hourly" ? 6 : 7);
  const step = Math.max(1, Math.ceil(dates.length / desired));

  const vals: number[] = [];
  for (let i = 0; i < dates.length; i += step) vals.push(dates[i]);

  const last = dates[dates.length - 1];
  if (vals[vals.length - 1] !== last) vals.push(last);

  return vals;
});

const tooltipLabelFormatter = (d: any) => formatForGrain(+new Date(d));

const crosshairTemplate = computed(() => {
  const tpl = componentToString(chartConfig.value, ChartTooltipContent, {
    labelFormatter: tooltipLabelFormatter,
  });

  return (d: any) => (d ? tpl(d) : "");
});

const hasData = computed(
  () =>
    normalized.value.length > 0 &&
    normalized.value.some((d) =>
      props.series.some((s) => (d as any)[s.key] > 0)
    )
);
</script>

<template>
  <div v-if="hasData">
    <ChartContainer
      :config="chartConfig"
      class="aspect-auto w-full"
      :style="{ height: `${height}px` }"
      :cursor="false"
    >
      <VisXYContainer
        :data="normalized"
        :svg-defs="svgDefs"
        :margin="{ left: -40 }"
        :y-domain="derivedYDomain"
        :duration="0"
      >
        <VisArea
          :x="(d: any) => d.date"
          :y="
            stacked
              ? stackedYFns.area
              : props.series.map((s) => (d: any) => d[s.key])
          "
          :color="(d: any, i: number) => areaColors[i]"
          :opacity="0.6"
        />

        <VisLine
          :x="(d: any) => d.date"
          :y="
            stacked
              ? stackedYFns.line
              : props.series.map((s) => (d: any) => d[s.key])
          "
          :color="(d: any, i: number) => lineColors[i]"
          :line-width="1"
        />

        <VisAxis
          type="x"
          :x="(d: any) => d.date"
          :tick-line="false"
          :domain-line="false"
          :grid-line="false"
          :tick-values="xTickValues"
          :tick-format="(ms: number) => formatForGrain(ms)"
        />
        <VisAxis
          type="y"
          :num-ticks="3"
          :tick-line="false"
          :domain-line="false"
        />

        <ChartTooltip />

        <ChartCrosshair
          :template="crosshairTemplate"
          :color="(d: any, i: number) => lineColors[i % lineColors.length]"
        />
      </VisXYContainer>

      <ChartLegendContent />
    </ChartContainer>
  </div>
</template>
