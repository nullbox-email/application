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
type DashboardScope = "alias" | "mailbox" | "account" | "invalid";

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

const normalized = computed(() => {
  const mailboxId = props.mailboxId?.trim() || null;
  const aliasId = props.aliasId?.trim() || null;
  return { mailboxId, aliasId };
});

const scope = computed<DashboardScope>(() => {
  const { mailboxId, aliasId } = normalized.value;

  if (aliasId) return mailboxId ? "alias" : "invalid";
  if (mailboxId) return "mailbox";
  return "account";
});

const requestKey = computed(() => {
  const { mailboxId, aliasId } = normalized.value;

  switch (scope.value) {
    case "alias":
      return `${mailboxId}:alias:${aliasId}:${props.type}:${props.number}`;
    case "mailbox":
      return `${mailboxId}:mailbox:${props.type}:${props.number}`;
    case "account":
      return `account:${props.type}:${props.number}`;
    default:
      return null;
  }
});

const requestParams = computed(() => {
  const { mailboxId, aliasId } = normalized.value;

  if (scope.value === "alias") {
    return {
      aliasId: aliasId!,
      mailboxId: mailboxId!,
      number: props.number,
      type: props.type,
    };
  }

  if (scope.value === "mailbox") {
    return {
      mailboxId: mailboxId!,
      number: props.number,
      type: props.type,
    };
  }

  if (scope.value === "account") {
    return {
      number: props.number,
      type: props.type,
    };
  }

  return null;
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

async function loadDashboard(key: string, params: any) {
  const seq = ++loadSeq;

  error.value = null;
  loadedKey.value = null;
  chart.value = [];
  messages.value = [];
  loading.value = true;

  try {
    const result = await getDashboard("v1", params);

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
  () => ({
    active: props.active,
    key: requestKey.value,
    params: requestParams.value,
    scope: scope.value,
  }),
  ({ active, key, params, scope }) => {
    if (!active) return;

    if (scope === "invalid") {
      error.value = "aliasId requires mailboxId.";
      chart.value = [];
      messages.value = [];
      loadedKey.value = null;
      return;
    }

    if (!key || !params) return;
    if (loadedKey.value === key) return;

    void loadDashboard(key, params);
  },
  { immediate: true }
);

const totalMessages = computed(() => messages.value.length);

const droppedMessages = computed(
  () => messages.value.filter((m) => m.messageOutcome === "Dropped").length
);

const forwardedMessages = computed(
  () => messages.value.filter((m) => m.messageOutcome === "Forwarded").length
);

const quarantinedMessages = computed(
  () => messages.value.filter((m) => m.messageOutcome === "Quarantined").length
);
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
        class="mt-6 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4"
      >
        <div
          v-for="n in 4"
          :key="n"
          class="h-32 animate-pulse rounded bg-muted"
        ></div>
      </div>

      <div
        v-else
        class="mt-6 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4"
      >
        <Card class="w-full">
          <CardHeader>
            <CardTitle class="text-2xl font-semibold tabular-nums">
              {{ totalMessages }}
            </CardTitle>
            <CardDescription>Total</CardDescription>
            <CardAction>
              <icon name="heroicons:envelope-solid" class="text-4xl text-indigo-500" />
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
              <icon name="heroicons:check-circle-solid" class="text-4xl text-emerald-500" />
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
              <icon name="heroicons:x-circle-solid" class="text-4xl text-red-500" />
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
                name="heroicons:shield-exclamation-solid"
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
