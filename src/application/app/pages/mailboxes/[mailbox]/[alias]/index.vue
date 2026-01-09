<script setup lang="ts">
import { toast } from "vue-sonner";
const { t } = useI18n({ useScope: "local" });

const route = useRoute();
const aliasProperty = computed(() => route.params.alias as string);
const mailboxProperty = computed(() => route.params.mailbox as string);

const { get, update } = useAliases();

const alias = ref<any | null>(null);

const loadingAlias = ref(false);
const loadError = ref<string | null>(null);

let loadSeq = 0;

const showAliasSkeleton = computed(() => loadingAlias.value && !alias.value);

const fullEmail = computed(() => {
  const local = alias.value?.localPart ?? aliasProperty.value;
  return `${local}@${mailboxProperty.value}`;
});

const copied = ref(false);

async function copyAlias() {
  try {
    await navigator.clipboard.writeText(fullEmail.value);
    copied.value = true;
    window.setTimeout(() => (copied.value = false), 2500);
  } catch {
    // optional: toast or fallback
  }
}

async function loadAlias() {
  const seq = ++loadSeq;

  loadError.value = null;
  alias.value = null;

  loadingAlias.value = true;
  try {
    const a = await get("v1", {
      id: aliasProperty.value,
      mailboxId: mailboxProperty.value,
    });

    if (seq !== loadSeq) return; // stale result

    // store UI-inverted value alongside API value
    // directPassthroughEnabled (API): true => bypass checks
    // quarantineAndChecksEnabled (UI): true => checks applied
    alias.value = a ?? null;
    if (alias.value) {
      alias.value.quarantineAndChecksEnabled = !(
        alias.value.directPassthroughEnabled ?? false
      );
    }
  } catch (e: any) {
    if (seq !== loadSeq) return;
    loadError.value = e?.data?.detail ?? e?.statusMessage ?? t("submit.error");
  } finally {
    if (seq === loadSeq) loadingAlias.value = false;
  }
}

onMounted(loadAlias);
watch([aliasProperty, mailboxProperty], loadAlias);

const updateAlias = async ($event: boolean) => {
  if (!alias.value) return;

  // optimistic UI
  alias.value.isEnabled = $event;

  await update(
    "v1",
    alias.value.id,
    alias.value.mailboxId,
    {
      id: alias.value.id,
      mailboxId: alias.value.mailboxId,
      name: alias.value.name,
      isEnabled: alias.value.isEnabled,
    },
    {
      onResponse: async () => {
        toast.success(t("submit.success"));
      },
      onResponseError: async (ctx) => {
        toast.error(ctx?.response?._data?.detail ?? t("submit.error"));
      },
    }
  ).catch(() => {});
};

// UI-inverted: when UI switch is ON => protections applied => API directPassthroughEnabled=false
const updateQuarantineAndChecks = async ($event: boolean) => {
  if (!alias.value) return;

  // optimistic UI
  alias.value.quarantineAndChecksEnabled = $event;
  alias.value.directPassthroughEnabled = !$event;

  await update(
    "v1",
    alias.value.id,
    alias.value.mailboxId,
    {
      id: alias.value.id,
      mailboxId: alias.value.mailboxId,
      name: alias.value.name,
      isEnabled: alias.value.isEnabled,
      directPassthroughEnabled: alias.value.directPassthroughEnabled,
    },
    {
      onResponse: async () => {
        toast.success(t("submit.success"));
      },
      onResponseError: async (ctx) => {
        toast.error(ctx?.response?._data?.detail ?? t("submit.error"));
      },
    }
  ).catch(() => {});
};
</script>

<template>
  <div>
    <page-heading :title="t('page.title', { alias: aliasProperty })">
      <div class="flex items-center space-x-4">
        <Button as-child>
          <nuxt-link :to="`${route.fullPath}/edit`">
            {{ t("actions.edit") }}
          </nuxt-link>
        </Button>
      </div>
    </page-heading>

    <!-- error -->
    <div
      v-if="loadError"
      class="mt-6 max-w-3xl rounded-md border border-destructive p-4 text-destructive"
    >
      {{ loadError }}
    </div>

    <!-- alias skeleton -->
    <div v-if="showAliasSkeleton" class="mt-6 max-w-3xl space-y-4">
      <div class="rounded-md bg-muted/50 p-4">
        <div class="h-5 w-64 animate-pulse rounded bg-muted"></div>
        <div class="mt-4 flex items-center justify-center gap-2">
          <div class="h-8 w-56 animate-pulse rounded bg-muted"></div>
          <div class="h-8 w-8 animate-pulse rounded bg-muted"></div>
        </div>
        <div class="mx-auto mt-3 h-4 w-40 animate-pulse rounded bg-muted"></div>
      </div>

      <div class="rounded-md border p-4">
        <div class="h-5 w-32 animate-pulse rounded bg-muted"></div>
        <div class="mt-2 h-4 w-64 animate-pulse rounded bg-muted"></div>
        <div class="ml-auto mt-4 h-9 w-14 animate-pulse rounded bg-muted"></div>
      </div>
    </div>

    <!-- alias content -->
    <div
      v-else-if="alias"
      class="mt-6 md:flex md:w-full gap-4 text-muted-foreground"
    >
      <!-- Left: email/copy card -->
      <div class="flex-1 min-w-0">
        <div class="w-full rounded-md h-full bg-muted/50 p-4">
          <p class="flex items-center text-lg font-medium">
            {{ t("intro") }}
          </p>

          <div
            class="my-2 flex flex-wrap items-center justify-center pt-4 font-mono text-lg md:text-2xl font-bold text-foreground"
          >
            <span class="py-1">{{
              alias.localPart ?? route.params.alias
            }}</span>
            <span class="px-1 py-1 font-medium">@</span>
            <span class="py-1">{{ mailboxProperty }}</span>

            <Button
              size="icon"
              variant="ghost"
              class="ml-2 cursor-pointer"
              :disabled="!alias"
              :aria-label="copied ? t('actions.copied') : t('actions.copy')"
              @click="copyAlias"
            >
              <icon
                v-if="copied"
                name="lucide:check"
                class="text-primary"
                aria-hidden="true"
              />
              <icon v-else name="lucide:copy" aria-hidden="true" />
            </Button>
          </div>

          <p v-if="copied" class="text-center text-sm">
            {{ t("copy.confirmation") }}
          </p>
        </div>
      </div>

      <!-- Right: status/switch card -->
      <div class="mt-6 md:mt-0 flex-1 min-w-0 space-y-2">
        <div
          class="flex w-full flex-row items-center justify-between gap-4 rounded-md border p-4"
          :class="[
            !alias.isEnabled
              ? 'border-destructive text-destructive'
              : 'text-foreground',
          ]"
        >
          <div class="flex items-center justify-between space-x-2">
            <icon
              v-if="alias.isEnabled"
              name="solar:shield-check-bold-duotone"
              class="text-4xl text-emerald-600"
            />
            <icon
              v-else
              name="solar:shield-cross-bold-duotone"
              class="text-4xl text-destructive"
            />
            <div class="space-y-0.5">
              <Label for="enabled" class="text-base">
                {{
                  alias.isEnabled
                    ? t("status.enabled.label")
                    : t("status.disabled.label")
                }}
              </Label>

              <div class="text-sm">
                {{
                  alias.isEnabled
                    ? t("status.enabled.description")
                    : t("status.disabled.description")
                }}
              </div>
            </div>
          </div>

          <Switch
            id="enabled"
            :model-value="alias.isEnabled"
            :aria-label="
              alias.isEnabled
                ? t('status.enabled.toggleAria')
                : t('status.disabled.toggleAria')
            "
            @update:model-value="updateAlias"
          />
        </div>

        <!-- UI-inverted passthrough: switch ON => protections applied -->
        <div
          class="flex w-full flex-row items-center justify-between gap-4 rounded-md border p-4"
          :class="[
            !alias.quarantineAndChecksEnabled
              ? 'border-destructive text-destructive'
              : 'text-foreground',
          ]"
        >
          <div class="flex items-center justify-between space-x-2">
            <icon
              v-if="alias.quarantineAndChecksEnabled"
              name="solar:chat-square-check-bold-duotone"
              class="text-4xl text-emerald-600"
            />
            <icon
              v-else
              name="solar:chat-square-arrow-bold-duotone"
              class="text-4xl text-destructive"
            />
            <div class="space-y-0.5">
              <Label for="quarantine_and_checks" class="text-base">
                {{
                  alias.quarantineAndChecksEnabled
                    ? t("passthrough.disabled.label")
                    : t("passthrough.enabled.label")
                }}
              </Label>

              <div class="text-sm">
                {{
                  alias.quarantineAndChecksEnabled
                    ? t("passthrough.disabled.description")
                    : t("passthrough.enabled.description")
                }}
              </div>
            </div>
          </div>

          <Switch
            id="quarantine_and_checks"
            :model-value="alias.quarantineAndChecksEnabled"
            :aria-label="
              alias.quarantineAndChecksEnabled
                ? t('passthrough.disabled.toggleAria')
                : t('passthrough.enabled.toggleAria')
            "
            @update:model-value="updateQuarantineAndChecks"
          />
        </div>
      </div>
    </div>

    <dashboard-activity
      active
      :alias-id="alias?.id"
      :mailbox-id="alias?.mailboxId"
      :number="30"
      type="Daily"
      :fallback-error="t('submit.error')"
      :series="[
        {
          key: 'dropped',
          label: t('chart.series.dropped'),
          color: 'var(--destructive)',
          fill: { id: 'fillDropped' },
        },
        {
          key: 'quarantined',
          label: t('chart.series.quarantined'),
          color: 'var(--warning)',
          fill: { id: 'fillQuarantined' },
        },
        {
          key: 'forwarded',
          label: t('chart.series.forwarded'),
          color: 'var(--chart-2)',
          fill: { id: 'fillForwarded' },
        },
      ]"
    />
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  page:
    title: "{alias} activity"

  intro: Use this alias to receive emails.

  actions:
    edit: Edit
    copy: Copy address
    copied: Address copied

  copy:
    confirmation: Copied to clipboard.

  status:
    enabled:
      label: Enabled
      description: Emails are forwarded.
      toggleAria: Disable forwarding
    disabled:
      label: Disabled
      description: Emails are not forwarded.
      toggleAria: Enable forwarding

  passthrough:
    enabled:
      label: Direct passthrough
      description: Bypasses quarantine and safety checks.
      toggleAria: Disable direct passthrough
    disabled:
      label: Quarantine and safety checks
      description: Quarantine and safety checks are applied.
      toggleAria: Enable quarantine and safety checks

  chart:
    series:
      total: Total
      forwarded: Forwarded
      dropped: Dropped
      quarantined: Quarantined
      delivered: Delivered
      failed: Failed

  submit:
    label: Save changes
    success: Alias updated.
    error: Could not update alias.
</i18n>
