<script setup lang="ts">
import { toast } from "vue-sonner";
import { toTypedSchema } from "@vee-validate/zod";
import { useForm } from "vee-validate";
import * as z from "zod";

const { t } = useI18n({ useScope: "local" });

const route = useRoute();
const aliasProperty = route.params.alias as string;
const mailboxProperty = route.params.mailbox as string;

const alias = ref<any | null>(null);
const { get, update } = useAliases();

alias.value = await get("v1", {
  id: aliasProperty,
  mailboxId: mailboxProperty,
});

const saving = ref(false);
const copied = ref(false);

const formSchema = toTypedSchema(
  z.object({
    name: z
      .string({ required_error: t("validation.name_required") })
      .min(2, t("validation.name_min"))
      .max(150, t("validation.name_max")),
    local_part: z
      .string({ required_error: t("validation.local_part_required") })
      .min(1, t("validation.local_part_min"))
      .regex(/^[a-z0-9._-]+$/i, t("validation.local_part_pattern")),
    is_enabled: z.boolean().default(false),

    // UI fields (inverted vs API)
    quarantine_and_checks: z.boolean().default(true), // true => directPassthrough=false
    active_protection: z.boolean().default(true), // true => learningMode=false
  })
);

debugger;
const { handleSubmit, values, resetForm } = useForm({
  validationSchema: formSchema,
  initialValues: {
    name: alias.value?.name ?? "",
    local_part: alias.value?.localPart ?? "",
    is_enabled: alias.value?.isEnabled ?? false,

    // invert from API -> UI
    quarantine_and_checks: !(alias.value?.directPassthrough ?? false),
    active_protection: !(alias.value?.learningMode ?? false),
  },
});

const aliasPreview = computed(() => {
  const localPart = values.local_part?.trim();
  if (!localPart) return "";
  return `${localPart}@${mailboxProperty}`;
});

async function copyAliasPreview() {
  if (!aliasPreview.value) return;

  try {
    await navigator.clipboard.writeText(aliasPreview.value);
    copied.value = true;
    window.setTimeout(() => (copied.value = false), 2500);
  } catch {
    // optional: toast or fallback
  }
}

const onSubmit = handleSubmit(async (formValues) => {
  if (!alias.value) return;

  saving.value = true;

  try {
    await update(
      "v1",
      alias.value.id,
      alias.value.mailboxId,
      {
        id: alias.value.id,
        mailboxId: alias.value.mailboxId,
        name: formValues.name.trim(),
        isEnabled: formValues.is_enabled,

        // invert back to API values
        directPassthrough: !formValues.quarantine_and_checks,
        learningMode: !formValues.active_protection,
      },
      {
        onResponse: async (ctx) => {
          toast.success(t("submit.success"));

          // keep local state in sync for reset + any UI that reads alias.value
          alias.value.name = formValues.name.trim();
          alias.value.isEnabled = formValues.is_enabled;
          alias.value.directPassthrough = !formValues.quarantine_and_checks;
          alias.value.learningMode = !formValues.active_protection;

          await refreshMailboxes().catch(() => {});
          await navigateTo(
            `/mailboxes/${route.params.mailbox}/${ctx.response._data.value}`
          );
        },
        onResponseError: async (ctx) => {
          toast.error(ctx?.response?._data?.detail ?? t("submit.error"));
          saving.value = false;
        },
      }
    );
  } catch {
    toast.error(t("submit.error"));
  } finally {
    resetForm({
      values: {
        name: alias.value?.name ?? formValues.name.trim(),
        local_part: alias.value?.localPart ?? values.local_part,
        is_enabled: alias.value?.isEnabled ?? formValues.is_enabled,

        // reset UI fields from (possibly updated) alias state
        quarantine_and_checks: !(alias.value?.directPassthrough ?? false),
        active_protection: !(alias.value?.learningMode ?? false),
      },
    });
    saving.value = false;
  }
});
</script>

<template>
  <div>
    <page-heading :title="t('page.title', { alias: aliasProperty })" />

    <div class="mt-6 max-w-3xl space-y-2 text-muted-foreground">
      <div class="rounded-md bg-muted/50 p-4">
        <p class="flex items-center text-lg font-medium">
          {{ t("intro.line1") }}
        </p>

        <div
          class="my-2 flex flex-wrap items-baseline justify-center pt-4 font-mono text-2xl font-bold text-foreground"
        >
          <span class="py-1">
            {{ values.local_part }}
          </span>

          <span class="px-1 py-1 font-medium">@</span>
          <span class="py-1">{{ mailboxProperty }}</span>

          <Button
            v-if="aliasPreview"
            size="icon"
            variant="ghost"
            class="ml-2 cursor-pointer"
            :aria-label="copied ? t('actions.copied') : t('actions.copy')"
            @click="copyAliasPreview"
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

    <div class="mt-6 max-w-3xl">
      <form class="space-y-6" @submit="onSubmit">
        <FormField v-slot="{ componentField }" name="name">
          <FormItem>
            <FormLabel>{{ t("fields.name.label") }}</FormLabel>
            <FormControl>
              <Input
                type="text"
                v-bind="componentField"
                :disabled="saving"
                autocomplete="off"
              />
            </FormControl>
            <FormDescription>
              {{ t("fields.name.description") }}
            </FormDescription>
            <FormMessage />
          </FormItem>
        </FormField>

        <FormField v-slot="{ componentField }" name="local_part">
          <FormItem>
            <FormLabel>{{ t("fields.local_part.label") }}</FormLabel>
            <FormControl>
              <Input
                type="text"
                v-bind="componentField"
                autocomplete="off"
                readonly
                disabled
              />
            </FormControl>
            <FormDescription>
              {{
                t("fields.local_part.description", { domain: mailboxProperty })
              }}
            </FormDescription>
            <FormMessage />
          </FormItem>
        </FormField>

        <!-- integrated: same enabled/disabled language as dashboard -->
        <FormField v-slot="{ value, handleChange }" name="is_enabled">
          <FormItem
            class="flex flex-row items-center justify-between rounded-lg border p-4"
            :class="[!value ? 'border-destructive text-destructive' : '']"
          >
            <div class="flex items-center justify-between space-x-2">
              <icon
                v-if="value"
                name="solar:shield-check-bold-duotone"
                class="text-4xl text-emerald-600"
              />
              <icon
                v-else
                name="solar:shield-cross-bold-duotone"
                class="text-4xl text-destructive"
              />
              <div class="space-y-0.5">
                <FormLabel class="text-base">
                  {{
                    value ? t("status.enabled.label") : t("status.disabled.label")
                  }}
                </FormLabel>

                <FormDescription :class="[!value ? 'text-destructive' : '']">
                  {{
                    value
                      ? t("status.enabled.description")
                      : t("status.disabled.description")
                  }}
                </FormDescription>
              </div>
            </div>

            <FormControl>
              <Switch
                :model-value="value"
                :disabled="saving"
                :aria-label="
                  value
                    ? t('status.enabled.toggleAria')
                    : t('status.disabled.toggleAria')
                "
                @update:model-value="handleChange"
              />
            </FormControl>
          </FormItem>
        </FormField>

        <!-- Inverted UI: when ON => protections applied (API directPassthrough=false) -->
        <FormField
          v-slot="{ value, handleChange }"
          name="quarantine_and_checks"
        >
          <FormItem
            class="flex flex-row items-center justify-between rounded-lg border p-4"
            :class="[!value ? 'border-destructive text-destructive' : '']"
          >
            <div class="flex items-center justify-between space-x-2">
              <icon
                v-if="value"
                name="solar:shield-check-bold-duotone"
                class="text-4xl text-emerald-600"
              />
              <icon
                v-else
                name="solar:shield-cross-bold-duotone"
                class="text-4xl text-destructive"
              />
              <div class="space-y-0.5">
                <Label class="text-base">
                  {{
                    value
                      ? t("passthrough.disabled.label")
                      : t("passthrough.enabled.label")
                  }}
                </Label>

                <div class="text-sm">
                  {{
                    value
                      ? t("passthrough.disabled.description")
                      : t("passthrough.enabled.description")
                  }}
                </div>
              </div>
            </div>

            <FormControl>
              <Switch
                :model-value="value"
                :disabled="saving"
                :aria-label="
                  value
                    ? t('passthrough.disabled.toggleAria')
                    : t('passthrough.enabled.toggleAria')
                "
                @update:model-value="handleChange"
              />
            </FormControl>
          </FormItem>
        </FormField>

        <div>
          <h3 class="font-semibold">{{ t("active_protection.title") }}</h3>
          <p class="text-sm text-muted-foreground">
            {{ t("active_protection.description") }}
          </p>
        </div>

        <!-- Inverted UI: when ON => learning is OFF (API learningMode=false) -->
        <FormField v-slot="{ value, handleChange }" name="active_protection">
          <FormItem
            class="flex flex-row items-center justify-between rounded-lg border p-4"
            :class="[!value ? 'border-destructive text-destructive' : '']"
          >
            <div class="flex items-center justify-between space-x-2">
              <icon
                v-if="value"
                name="solar:shield-check-bold-duotone"
                class="text-4xl text-emerald-600"
              />
              <icon
                v-else
                name="solar:chat-square-arrow-bold-duotone"
                class="text-4xl text-destructive"
              />
              <div class="space-y-0.5">
                <Label class="text-base">
                  {{
                    value
                      ? t("learning.disabled.label")
                      : t("learning.enabled.label")
                  }}
                </Label>

                <div class="text-sm">
                  {{
                    value
                      ? t("learning.disabled.description")
                      : t("learning.enabled.description")
                  }}
                </div>
              </div>
            </div>

            <FormControl>
              <Switch
                :model-value="value"
                :disabled="saving"
                :aria-label="
                  value
                    ? t('learning.disabled.toggleAria')
                    : t('learning.enabled.toggleAria')
                "
                @update:model-value="handleChange"
              />
            </FormControl>
          </FormItem>
        </FormField>

        <Button class="cursor-pointer" type="submit" :disabled="saving">
          <icon
            v-if="saving"
            name="lucide:loader-circle"
            class="animate-spin"
            aria-hidden="true"
          />
          <span v-else>{{ t("submit.label") }}</span>
        </Button>
      </form>
    </div>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  page:
    title: "Edit alias: {alias}"

  intro:
    line1: Update alias settings.

  actions:
    copy: Copy address
    copied: Address copied

  copy:
    confirmation: Copied to clipboard.

  fields:
    name:
      label: Alias name
      description: Use a name you can recognize later.

    local_part:
      label: Email prefix
      description: "This value cannot be changed. Domain: {domain}"

  # verbatim-aligned with dashboard "status" block
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

  active_protection:
    title: Active protection
    description: Learning mode helps identify trusted senders over time. When enabled, quarantine and safety checks are enforced immediately. New aliases start in learning mode for 30 days by default.

  learning:
    enabled:
      label: Learning mode
      description: Deliver emails and learn which senders are trusted for this alias.
      toggleAria: Disable learning mode
    disabled:
      label: Active protection
      description: Quarantine and safety checks are applied before delivery.
      toggleAria: Enable learning mode

  validation:
    name_required: Enter a name.
    name_min: Use at least 2 characters.
    name_max: Use fewer than 150 characters.
    local_part_required: Enter an email prefix.
    local_part_min: Use at least 1 character.
    local_part_pattern: Use letters, numbers, dots, underscores, or dashes.

  submit:
    label: Save changes
    success: Alias updated.
    error: Could not update alias.
</i18n>
