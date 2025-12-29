<script setup lang="ts">
type SeriesSpec = {
  key: string;
  label: string;
  color: string;
  fill?: {
    id: string;
    fromOpacity?: number;
    toOpacity?: number;
  };
};

type TimeGrain = "Hourly" | "Daily";

const props = withDefaults(
  defineProps<{
    aliasId?: string | null;
    mailboxId?: string | null;
    active?: boolean;
    number: number;
    type: TimeGrain;
    grain?: TimeGrain;
    series: SeriesSpec[];
    fallbackError?: string;
  }>(),
  {
    active: false,
    grain: undefined,
    fallbackError: "Something went wrong.",
  }
);

const { getDashboard } = useDashboards();

const chart = ref<any[]>([]);
const messages = ref<any[]>([]);
const loading = ref(false);
const error = ref<string | null>(null);

let loadSeq = 0;
const loadedKey = ref<string | null>(null);

const requestKey = computed(() => {
  const mailboxId = props.mailboxId?.trim();
  if (!mailboxId) return null;

  const aliasId = props.aliasId?.trim();
  const scope = aliasId ? `alias:${aliasId}` : "mailbox";
  return `${mailboxId}:${scope}:${props.type}:${props.number}`;
});

const chartGrain = computed<TimeGrain>(() => props.grain ?? props.type);

const showChartSkeleton = computed(
  () => props.active && (loading.value || !requestKey.value)
);

const showMessagesSkeleton = computed(
  () =>
    props.active &&
    (loading.value || !requestKey.value) &&
    messages.value.length === 0
);

async function loadDashboard(key: string) {
  const seq = ++loadSeq;

  error.value = null;
  loadedKey.value = null;
  chart.value = [];
  messages.value = [];
  loading.value = true;

  try {
    const aliasId = props.aliasId?.trim();
    const mailboxId = props.mailboxId?.trim();

    const result = await getDashboard("v1", {
      aliasId: aliasId || undefined,
      mailboxId: mailboxId || undefined,
      number: props.number,
      type: props.type,
    });

    if (seq !== loadSeq) return;

    chart.value = result?.chart ?? [];
    messages.value = result?.messages ?? [];
    loadedKey.value = key;
  } catch (e: any) {
    if (seq !== loadSeq) return;
    error.value = e?.data?.detail ?? e?.statusMessage ?? props.fallbackError;
  } finally {
    if (seq === loadSeq) loading.value = false;
  }
}

watch(
  () => ({ active: props.active, key: requestKey.value }),
  ({ active, key }) => {
    if (!active || !key) return;
    if (loadedKey.value === key) return;
    void loadDashboard(key);
  },
  { immediate: true }
);

const totalMessages = computed(() => {
  return messages.value.length;
});

const droppedMessages = computed(() => {
  return messages.value.filter((m) => m.messageOutcome === "Dropped").length;
});

const forwardedMessages = computed(() => {
  return messages.value.filter((m) => m.messageOutcome === "Forwarded").length;
});

const quarantinedMessages = computed(() => {
  return messages.value.filter((m) => m.messageOutcome === "Quarantined")
    .length;
});
</script>

<template>
  <div>
    <div
      v-if="error"
      class="mt-6 rounded-md border border-destructive p-4 text-destructive"
    >
      {{ error }}
    </div>

    <template v-if="props.active || loadedKey">
      <div
        v-if="showChartSkeleton || showMessagesSkeleton"
        class="flex space-x-4"
      >
        <div
          v-for="n in 4"
          :key="n"
          class="mt-6 h-32 w-full animate-pulse rounded bg-muted"
        ></div>
      </div>

      <div v-else class="mt-6 flex space-x-4">
        <Card class="w-full">
          <CardHeader>
            <CardTitle class="text-2xl font-semibold tabular-nums">
              {{ totalMessages }}
            </CardTitle>
            <CardDescription>Total</CardDescription>
            <CardAction>
              <icon name="lucide:mail" class="text-4xl text-indigo-500" />
            </CardAction>
          </CardHeader>
        </Card>
        <Card class="w-full">
          <CardHeader>
            <CardTitle class="text-2xl font-semibold tabular-nums">
              {{ forwardedMessages }}
            </CardTitle>
            <CardDescription>Forwarded</CardDescription>
            <CardAction>
              <icon name="lucide:send" class="text-4xl text-blue-500" />
            </CardAction>
          </CardHeader>
        </Card>
        <Card class="w-full">
          <CardHeader>
            <CardTitle class="text-2xl font-semibold tabular-nums">
              {{ droppedMessages }}
            </CardTitle>
            <CardDescription>Dropped</CardDescription>
            <CardAction>
              <icon name="lucide:mail-x" class="text-4xl text-red-500" />
            </CardAction>
          </CardHeader>
        </Card>
        <Card class="w-full">
          <CardHeader>
            <CardTitle class="text-2xl font-semibold tabular-nums">
              {{ quarantinedMessages }}
            </CardTitle>
            <CardDescription>Quarantined</CardDescription>
            <CardAction>
              <icon
                name="lucide:shield-alert"
                class="text-4xl text-amber-500"
              />
            </CardAction>
          </CardHeader>
        </Card>
      </div>

      <div v-if="showChartSkeleton" class="mt-6">
        <div class="mt-4 h-60 w-full animate-pulse rounded bg-muted"></div>
      </div>

      <dashboard-chart
        v-else
        class="mt-6"
        :data="chart"
        :grain="chartGrain"
        :series="props.series"
      />

      <div v-if="showMessagesSkeleton" class="mt-6">
        <div v-for="n in 10" :key="n" class="px-2 py-1">
          <div class="h-8 w-full animate-pulse rounded bg-muted"></div>
        </div>
      </div>

      <dashboard-messages v-else class="mt-6" :messages="messages" />
    </template>
  </div>
</template>
