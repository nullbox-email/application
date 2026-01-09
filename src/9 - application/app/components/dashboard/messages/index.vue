<script setup lang="ts">
import { computed } from "vue";

export type DeliveryDecision =
  | "Delivered"
  | "Dropped"
  | "Forwarded"
  | "Quarantined"
  | "Unknown";

export type ProviderStatus = "Pending" | "Succeeded" | "Failed" | "Unknown";

export type RecentDeliveryMessage = {
  id: string;
  deliveryActionId: string;
  receivedAt: string | Date;

  senderDisplay: string;
  senderDomain: string;
  subject: string;

  hasAttachments?: boolean;
  attachmentsCount?: number;

  // Aligned with API/DTO: messageOutcome (not deliveryDecision)
  messageOutcome: DeliveryDecision;
  reason?: string;

  // Nullable again (null = not attempted / not applicable)
  providerStatus?: ProviderStatus | null;
};

const props = withDefaults(
  defineProps<{
    messages: RecentDeliveryMessage[];
    showProviderStatus?: boolean;
    showTime?: boolean;
    subjectClamp?: 1 | 2;
    to?: (m: RecentDeliveryMessage) => string | undefined;
  }>(),
  {
    showProviderStatus: true,
    showTime: true,
    subjectClamp: 1,
  }
);

function toDate(d: string | Date | null | undefined) {
  if (!d) return null;
  return typeof d === "string" ? new Date(d) : d;
}

function formatSender(m: RecentDeliveryMessage) {
  const name = (m.senderDisplay || "").trim();
  const domain = (m.senderDomain || "").trim();
  if (name) return name;
  if (domain) return domain;
  return "Unknown sender";
}

function formatSubject(m: RecentDeliveryMessage) {
  const s = (m.subject || "").trim();
  return s || "(no subject)";
}

function timeAgo(d: Date) {
  const diffMs = Date.now() - d.getTime();
  const sec = Math.max(0, Math.floor(diffMs / 1000));
  const min = Math.floor(sec / 60);
  const hr = Math.floor(min / 60);
  const day = Math.floor(hr / 24);

  if (day > 0) return `${day}d`;
  if (hr > 0) return `${hr}h`;
  if (min > 0) return `${min}m`;
  return `${sec}s`;
}

const decisionRail = computed(() => [
  {
    key: "Dropped" as const,
    label: "Dropped",
    icon: "heroicons:x-circle-solid",
    class: "text-red-500",
  },
  {
    key: "Forwarded" as const,
    label: "Forwarded",
    icon: "heroicons:check-circle-solid",
    class: "text-emerald-500",
  },
  {
    key: "Quarantined" as const,
    label: "Quarantined",
    icon: "heroicons:shield-exclamation-solid",
    class: "text-amber-500",
  },
  {
    key: "Delivered" as const,
    label: "Delivered",
    icon: "heroicons:check-circle-solid",
    class: "text-emerald-500",
  },
]);

function isActiveDecision(m: RecentDeliveryMessage, k: DeliveryDecision) {
  return (m.messageOutcome || "Unknown") === k;
}

function shouldShowDecisionTooltip(m: RecentDeliveryMessage) {
  const o = (m.messageOutcome || "Unknown") as DeliveryDecision;
  return o === "Quarantined" || o === "Dropped";
}

function decisionTooltipText(m: RecentDeliveryMessage) {
  const o = (m.messageOutcome || "Unknown") as DeliveryDecision;
  if (o === "Quarantined") return m.reason;
  if (o === "Dropped") return m.reason;
  return "";
}

function providerDotClass(m: RecentDeliveryMessage) {
  const s = m.providerStatus?.toLowerCase();

  // null/undefined = no provider outcome yet (or not applicable)
  if (!s) return "bg-muted-foreground/30";

  if (s === "succeeded") return "bg-emerald-500/80";
  if (s === "failed") return "bg-rose-500/80";
  if (s === "pending") return "bg-amber-500/80";
  return "bg-muted-foreground/70";
}

function hasAttach(m: RecentDeliveryMessage) {
  if (typeof m.hasAttachments === "boolean") return m.hasAttachments;
  if (typeof m.attachmentsCount === "number") return m.attachmentsCount > 0;
  return false;
}
</script>

<template>
  <div v-if="messages.length" class="min-w-0">
    <ul role="list" class="divide-y divide-border">
      <li v-for="m in messages" :key="m.id ?? m.deliveryActionId">
        <nuxt-link
          :to="to?.(m)"
          class="flex items-center justify-between gap-x-3 px-2 py-3 hover:bg-accent/50 rounded-md"
          :class="to?.(m) ? 'cursor-pointer' : ''"
        >
          <div class="min-w-0 flex-1">
            <div class="flex min-w-0 items-center gap-x-2">
              <p class="truncate text-sm font-medium text-foreground">
                {{ formatSender(m) }}
              </p>

              <span class="text-muted-foreground/40">•</span>

              <p
                class="min-w-0 flex-1 text-sm text-muted-foreground"
                :class="subjectClamp === 2 ? 'line-clamp-2' : 'truncate'"
              >
                {{ formatSubject(m) }}
              </p>

              <span
                v-if="hasAttach(m)"
                class="ml-1 inline-flex flex-none items-center text-muted-foreground"
                title="Has attachments"
                aria-label="Has attachments"
              >
                <Icon name="solar:paperclip-bold" class="text-xl" />
              </span>
            </div>
          </div>

          <div class="flex shrink-0 items-center gap-x-3">
            <div class="flex items-center" aria-label="Delivery decision">
              <template v-for="d in decisionRail" :key="d.key">
                <template v-if="isActiveDecision(m, d.key)">
                  <!-- Tooltip only for Quarantined/Dropped -->
                  <TooltipProvider v-if="shouldShowDecisionTooltip(m)">
                    <Tooltip>
                      <TooltipTrigger as-child>
                        <span
                          class="inline-flex items-center text-muted-foreground"
                        >
                          <Icon
                            :name="d.icon"
                            class="text-xl"
                            :class="d.class"
                          />
                        </span>
                      </TooltipTrigger>
                      <TooltipContent>
                        <p>{{ decisionTooltipText(m) }}</p>
                      </TooltipContent>
                    </Tooltip>
                  </TooltipProvider>

                  <!-- No tooltip for other outcomes -->
                  <span
                    v-else
                    class="inline-flex items-center text-muted-foreground"
                  >
                    <Icon :name="d.icon" class="text-xl" :class="d.class" />
                  </span>
                </template>
              </template>
            </div>

            <div v-if="showProviderStatus" class="flex items-center">
              <span class="flex-none rounded-full bg-muted p-1">
                <span
                  class="block size-1.5 rounded-full"
                  :class="providerDotClass(m)"
                />
              </span>
            </div>

            <p
              v-if="showTime"
              class="hidden sm:block text-xs text-muted-foreground text-right min-w-6"
            >
              <time
                v-if="toDate(m.receivedAt)"
                :datetime="toDate(m.receivedAt)!.toISOString()"
              >
                {{ timeAgo(toDate(m.receivedAt)!) }}
              </time>
            </p>
          </div>
        </nuxt-link>
      </li>
    </ul>
  </div>
  <div v-else class="text-center">
    <svg
      class="mx-auto size-12 text-gray-400"
      xmlns="http://www.w3.org/2000/svg"
      width="24"
      height="24"
      viewBox="0 0 24 24"
    >
      <path
        fill="none"
        stroke="currentColor"
        stroke-linecap="round"
        stroke-linejoin="round"
        stroke-width="1.5"
        d="M21.75 6.75v10.5a2.25 2.25 0 0 1-2.25 2.25h-15a2.25 2.25 0 0 1-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0 0 19.5 4.5h-15a2.25 2.25 0 0 0-2.25 2.25m19.5 0v.243a2.25 2.25 0 0 1-1.07 1.916l-7.5 4.615a2.25 2.25 0 0 1-2.36 0L3.32 8.91a2.25 2.25 0 0 1-1.07-1.916V6.75"
      />
    </svg>
    <h3 class="mt-2 text-sm font-semibold text-foreground">No emails</h3>
    <p class="mt-1 text-sm text-muted-foreground">
      You haven’t received any emails during this period.
    </p>
  </div>
</template>
