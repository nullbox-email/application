<script setup lang="ts">
import { computed, nextTick, ref, watch } from "vue";
import { toast } from "vue-sonner";
import { toTypedSchema } from "@vee-validate/zod";
import { useForm } from "vee-validate";
import * as z from "zod";
import type { GetMailboxResponse } from "~/types/dto/mailbox/get-mailbox-response";

const { t } = useI18n({ useScope: "local" });

const props = defineProps<{
  mailbox: GetMailboxResponse;
}>();

const { create } = useAliases();
const { refreshMailboxes } = useGlobalMailboxes();

const routingHost = computed(
  () => `${props.mailbox.routingKey}.${props.mailbox.domain}`
);

const autoCreateOn = computed(() => !!props.mailbox.autoCreateAlias);

/** mailbox-domain copy (existing) */
const copied = ref(false);
async function copyRoutingHost() {
  try {
    await navigator.clipboard.writeText(`@${routingHost.value}`);
    copied.value = true;
    window.setTimeout(() => (copied.value = false), 2500);
  } catch {
    // optional: toast or fallback
  }
}

/** embedded alias create (step 3 when autoCreate is off) */
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
  })
);

const { handleSubmit, values, resetForm, setValues } = useForm({
  validationSchema: formSchema,
  initialValues: { name: "", local_part: "" },
});

const localPartManuallyEdited = ref(false);
let syncingLocalPartFromName = false;

function slugifyLocalPart(value: string) {
  if (!value) return "";
  return value
    .normalize("NFKD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/[^a-zA-Z0-9._-]+/g, "-")
    .replace(/-{2,}/g, "-")
    .replace(/^[-.]+/, "")
    .replace(/[-.]+$/, "")
    .toLowerCase();
}

function syncLocalPartFromName(name: string) {
  syncingLocalPartFromName = true;
  setValues({ local_part: slugifyLocalPart(name) });
  nextTick(() => (syncingLocalPartFromName = false));
}

watch(
  () => values.name,
  (newName) => {
    if (autoCreateOn.value) return;
    if (localPartManuallyEdited.value) return;
    syncLocalPartFromName(newName ?? "");
  },
  { immediate: true }
);

watch(
  () => values.local_part,
  () => {
    if (autoCreateOn.value) return;
    if (syncingLocalPartFromName) return;
    localPartManuallyEdited.value = true;
  }
);

const aliasPreview = computed(() => {
  const localPart = values.local_part?.trim();
  if (!localPart) return "";
  return `${localPart}@${routingHost.value}`;
});

const isLocalPartLinked = computed(() => !localPartManuallyEdited.value);

function relinkLocalPart() {
  localPartManuallyEdited.value = false;
  syncLocalPartFromName(values.name ?? "");
}

const savingAlias = ref(false);
const aliasCopied = ref(false);

async function copyAliasPreview() {
  if (!aliasPreview.value) return;
  try {
    await navigator.clipboard.writeText(aliasPreview.value);
    aliasCopied.value = true;
    window.setTimeout(() => (aliasCopied.value = false), 2500);
  } catch {
    // optional: toast or fallback
  }
}

const onCreateAlias = handleSubmit(async (formValues) => {
  if (autoCreateOn.value) return;

  savingAlias.value = true;
  try {
    await create(
      "v1",
      props.mailbox.id,
      {
        mailboxId: props.mailbox.id,
        name: formValues.name.trim(),
        localPart: formValues.local_part.trim(),
      },
      {
        onResponse: async () => {
          toast.success(t("submit.success"));
          await refreshMailboxes().catch(() => {});
          resetForm();
        },
        onResponseError: async (ctx) => {
          toast.error(ctx?.response?._data?.detail ?? t("submit.error"));
          savingAlias.value = false;
          return;
        },
      }
    );
  } finally {
    savingAlias.value = false;
  }
});
</script>

<template>
  <div v-if="mailbox">
    <!-- intro / context -->
    <div class="mt-2 max-w-3xl space-y-2 text-muted-foreground">
      <div class="bg-muted/50 p-4 rounded-md">
        <p class="flex text-lg font-medium items-center">
          <icon
            name="solar:check-circle-bold"
            class="text-xl text-green-500 mr-2"
          />
          {{ t("intro.line1") }}
        </p>

        <div
          class="my-2 pt-4 flex flex-wrap items-baseline justify-center text-2xl font-mono font-bold text-foreground"
        >
          <span class="px-1 py-1 font-medium"> @ </span>
          <span class="py-1">
            {{ mailbox.routingKey }}
          </span>
          <span class="py-1 font-medium"> . </span>
          <span class="py-1 font-medium">
            {{ mailbox.domain }}
          </span>

          <Button
            size="icon"
            variant="ghost"
            class="cursor-pointer ml-2"
            @click="copyRoutingHost"
          >
            <icon v-if="copied" name="lucide:check" class="text-green-500" />
            <icon v-else name="lucide:copy" />
          </Button>
        </div>
      </div>

      <p class="text-base">
        {{ t("intro.line2") }}
      </p>
    </div>

    <div class="mt-6 space-y-8 md:max-w-3xl">
      <timeline>
        <!-- Step 1 -->
        <timeline-step
          :number="1"
          :title="t('steps.1.title')"
          lineFrom="md"
          class="pb-6"
        >
          <template #body>
            {{ t("steps.1.body") }}
          </template>
        </timeline-step>

        <!-- Step 2 -->
        <timeline-step
          :number="2"
          :title="t('steps.2.title')"
          lineFrom="md"
          class="pb-6"
        >
          <template #body>
            {{ t("steps.2.body") }}
          </template>
        </timeline-step>

        <!-- Step 3 -->
        <timeline-step
          :number="3"
          :title="t('steps.3.title')"
          lineFrom="md"
          class="pb-6"
        >
          <template #body>
            <div class="space-y-4">
              <p class="text-muted-foreground">
                {{ autoCreateOn ? t("steps.3.body_on") : t("steps.3.body_off") }}
              </p>

              <!-- auto-create ON: example only -->
              <div
                v-if="autoCreateOn"
                class="my-2 flex flex-wrap items-baseline gap-2 md:text-2xl bg-muted/50 p-4 rounded-md"
              >
                <div class="flex items-baseline font-mono">
                  <span
                    class="inline-flex justify-end md:w-[12ch] font-semibold"
                  >
                    <ContainerTextFlip
                      :words="[
                        'zoom',
                        'apple',
                        'slack',
                        'figma',
                        'steam',
                        'amazon',
                        'github',
                        'paypal',
                        'stripe',
                        'google',
                        'notion',
                        'spotify',
                        'netflix',
                        'dropbox',
                        'microsoft',
                      ]"
                    />
                  </span>

                  <span class="px-1">@</span>
                  <span class="py-1">{{ routingHost }}</span>
                </div>
              </div>

              <p v-if="autoCreateOn" class="text-muted-foreground/50">
                {{ t("steps.3.hint_on") }}
              </p>

              <!-- auto-create OFF: embedded alias create -->
              <div v-else class="space-y-4">
                <div class="bg-muted/50 p-4 rounded-md space-y-3">
                  <div
                    class="flex flex-wrap items-baseline justify-center gap-1 text-2xl font-mono font-bold text-foreground"
                  >
                    <span v-if="values.local_part" class="py-1">
                      {{ values.local_part }}
                    </span>
                    <span v-else class="py-1">
                      <ContainerTextFlip
                        :words="[
                          'zoom',
                          'apple',
                          'slack',
                          'figma',
                          'steam',
                          'amazon',
                          'github',
                          'paypal',
                          'stripe',
                          'google',
                          'notion',
                          'spotify',
                          'netflix',
                          'dropbox',
                          'microsoft',
                        ]"
                      />
                    </span>
                    <span class="px-1 py-1 font-medium">@</span>
                    <span class="py-1">{{ mailbox.routingKey }}</span>
                    <span class="py-1 font-medium">.</span>
                    <span class="py-1 font-medium">{{ mailbox.domain }}</span>

                    <Button
                      v-if="aliasPreview"
                      size="icon"
                      variant="ghost"
                      class="cursor-pointer ml-2"
                      @click="copyAliasPreview"
                    >
                      <icon
                        v-if="aliasCopied"
                        name="lucide:check"
                        class="text-green-500"
                      />
                      <icon v-else name="lucide:copy" />
                    </Button>
                  </div>

                  <p class="text-muted-foreground/50 text-sm text-center">
                    {{ t("steps.3.preview_hint") }}
                  </p>
                </div>

                <form class="space-y-6" @submit="onCreateAlias">
                  <FormField v-slot="{ componentField }" name="name">
                    <FormItem>
                      <FormLabel>{{ t("fields.name.label") }}</FormLabel>
                      <FormControl>
                        <Input
                          type="text"
                          v-bind="componentField"
                          :disabled="savingAlias"
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
                          :disabled="savingAlias"
                          autocomplete="off"
                        />
                      </FormControl>
                      <FormDescription>
                        {{
                          t("fields.local_part.description", {
                            domain: routingHost,
                          })
                        }}
                      </FormDescription>

                      <p
                        class="text-xs text-muted-foreground mt-1 flex items-center gap-2"
                      >
                        <span
                          v-if="!isLocalPartLinked"
                          class="flex items-center gap-2"
                        >
                          {{ t("fields.local_part.unlinked_hint") }}
                          <Button
                            type="button"
                            variant="link"
                            size="sm"
                            class="h-auto p-0 text-xs"
                            @click="relinkLocalPart"
                          >
                            {{ t("fields.local_part.relink") }}
                          </Button>
                        </span>
                        <span v-else class="opacity-60">
                          {{ t("fields.local_part.linked_hint") }}
                        </span>
                      </p>

                      <FormMessage />
                    </FormItem>
                  </FormField>

                  <Button
                    class="cursor-pointer"
                    type="submit"
                    :disabled="savingAlias"
                  >
                    <icon
                      v-if="savingAlias"
                      name="lucide:loader-circle"
                      class="animate-spin"
                    />
                    <div v-else>{{ t("submit.label") }}</div>
                  </Button>
                </form>

                <p class="text-muted-foreground/50">
                  {{ t("steps.3.hint_off") }}
                </p>
              </div>
            </div>
          </template>
        </timeline-step>

        <!-- Step 4 -->
        <timeline-step
          :number="4"
          :title="t('steps.4.title')"
          lineFrom="md"
          class="pb-6"
        >
          <template #body>
            {{ autoCreateOn ? t("steps.4.body_on") : t("steps.4.body_off") }}
          </template>
        </timeline-step>

        <!-- Step 5 -->
        <timeline-step
          :number="5"
          :title="t('steps.5.title')"
          lineFrom="md"
          :isLast="true"
        >
          <template #body>
            {{
              t("steps.5.body", {
                email: mailbox.emailAddress,
              })
            }}
          </template>
        </timeline-step>
      </timeline>
    </div>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  intro:
    line1: "You’re all set. This is your private mailbox domain."
    line2: "Next time a site asks for your email, use a unique Nullbox address instead of your real one."

  steps:
    "1":
      title: "What to do next"
      body: "Use Nullbox the next time you sign up for a service."
    "2":
      title: "Why this helps"
      body: "Using a different address per service reduces tracking and makes it easy to cut off spam later."

    "3":
      title: "How to use it"
      body_on: "Make up a name for the service and add it before your mailbox domain."
      hint_on: "Change the name before the {'@'} for each service."

      body_off: "Create an alias, then use it when you sign up."
      preview_hint: "This is the address you’ll use on the site."
      hint_off: "Create one alias per service."

    "4":
      title: "When Nullbox steps in"
      body_on: "As soon as a message is sent to that address, we create the alias automatically and start forwarding."
      body_off: "Forwarding starts after you’ve created the alias."

    "5":
      title: "Where your mail goes"
      body: "Emails are forwarded to {email}. Nullbox doesn’t replace your inbox."

  fields:
    name:
      label: Alias name
      description: Pick something that helps you recognize this alias later.
    local_part:
      label: Email prefix
      description: "We’ll append {'@'}{domain} after whatever you enter."
      linked_hint: This prefix follows the alias name. Edit it to customize.
      unlinked_hint: This prefix is custom now.
      relink: Match name again

  validation:
    name_required: Please provide a name.
    name_min: Name must be at least 2 characters long.
    name_max: Name must be fewer than 150 characters.
    local_part_required: Please provide an email prefix.
    local_part_min: Email prefix must include at least one character.
    local_part_pattern: Use letters, numbers, dots, underscores, or dashes only.

  submit:
    label: Create alias
    success: Your alias has been created successfully.
    error: Something went wrong while creating the alias.
</i18n>
