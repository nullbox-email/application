<script setup lang="ts">
import { toast } from "vue-sonner";
import { toTypedSchema } from "@vee-validate/zod";
import { useForm } from "vee-validate";
import * as z from "zod";
import type { GetResponse as GetMailboxResponse } from "~/types/dto/mailboxes/get-response";

const { t } = useI18n({
  useScope: "local",
});

const route = useRoute();

const { get } = useMailboxes();
const { create } = useAliases();
const { refreshMailboxes } = useGlobalMailboxes();

const mailbox = (await get("v1", {
  id: route.params.mailbox as string,
})) as GetMailboxResponse;

if (!mailbox) {
  throw new Error("Mailbox not found");
}

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
  initialValues: {
    name: "",
    local_part: "",
  },
});

const localPartManuallyEdited = ref(false);
let syncingLocalPartFromName = false;

function slugifyLocalPart(value: string) {
  if (!value) {
    return "";
  }

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

  setValues({
    local_part: slugifyLocalPart(name),
  });

  nextTick(() => {
    syncingLocalPartFromName = false;
  });
}

watch(
  () => values.name,
  (newName) => {
    if (localPartManuallyEdited.value) {
      return;
    }
    syncLocalPartFromName(newName ?? "");
  },
  { immediate: true }
);

watch(
  () => values.local_part,
  () => {
    if (syncingLocalPartFromName) {
      return;
    }
    localPartManuallyEdited.value = true;
  }
);

const aliasDomain = computed(() => `${mailbox.routingKey}.${mailbox.domain}`);

const aliasPreview = computed(() => {
  const localPart = values.local_part?.trim();
  if (!localPart) {
    return "";
  }
  return `${localPart}@${aliasDomain.value}`;
});

const isLocalPartLinked = computed(() => !localPartManuallyEdited.value);

const saving = ref(false);
const copied = ref(false);

async function copyAliasPreview() {
  if (!aliasPreview.value) {
    return;
  }

  try {
    await navigator.clipboard.writeText(aliasPreview.value);
    copied.value = true;
    window.setTimeout(() => (copied.value = false), 2500);
  } catch {
    // optional: toast or fallback
  }
}

const onSubmit = handleSubmit(async (formValues) => {
  saving.value = true;

  try {
    const alias = await create(
      "v1",
      mailbox.id,
      {
        mailboxId: mailbox.id,
        name: formValues.name.trim(),
        localPart: formValues.local_part.trim(),
      },
      {
        onResponse: async (ctx) => {
          toast.success(t("submit.success"));

          await refreshMailboxes().catch(() => {});
          await navigateTo(
            `/mailboxes/${route.params.mailbox}/${ctx.response._data.value}`
          );
        },
        onResponseError: async (ctx) => {
          toast.error(ctx?.response?._data?.detail ?? t("submit.error"));
          saving.value = false;
          return;
        },
      }
    );
  } catch (error: any) {
  } finally {
    resetForm();
    saving.value = false;
  }
});

function relinkLocalPart() {
  localPartManuallyEdited.value = false;
  syncLocalPartFromName(values.name ?? "");
}
</script>

<template>
  <div>
    <page-heading :title="t('page.title')" />

    <div class="mt-6 max-w-3xl space-y-2 text-muted-foreground">
      <div class="bg-muted/50 p-4 rounded-md">
        <p class="flex text-lg font-medium items-center">
          {{ t("intro.line1") }}
        </p>

        <div
          class="my-2 pt-4 flex flex-wrap items-baseline justify-center text-2xl font-mono font-bold text-foreground"
        >
          <span v-if="values.local_part" class="py-1">
            {{ values.local_part }}
          </span>
          <span v-else>
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
          /></span>
          <span class="px-1 py-1 font-medium">@</span>
          <span class="py-1">
            {{ mailbox.routingKey }}
          </span>
          <span class="py-1 font-medium">.</span>
          <span class="py-1 font-medium">
            {{ mailbox.domain }}
          </span>

          <Button
            v-if="aliasPreview"
            size="icon"
            variant="ghost"
            class="cursor-pointer ml-2"
            @click="copyAliasPreview"
          >
            <icon v-if="copied" name="lucide:check" class="text-green-500" />
            <icon v-else name="lucide:copy" />
          </Button>
        </div>
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
                autofocus
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
                :disabled="saving"
                autocomplete="off"
              />
            </FormControl>
            <FormDescription>
              {{ t("fields.local_part.description", { domain: aliasDomain }) }}
            </FormDescription>
            <p
              class="text-xs text-muted-foreground mt-1 flex items-center gap-2"
            >
              <span v-if="!isLocalPartLinked" class="flex items-center gap-2">
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
            </p>
            <FormMessage />
          </FormItem>
        </FormField>

        <Button class="cursor-pointer" type="submit" :disabled="saving">
          <icon
            v-if="saving"
            name="lucide:loader-circle"
            class="animate-spin"
          />
          <div v-else>
            {{ t("submit.label") }}
          </div>
        </Button>
      </form>
    </div>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  page:
    title: Create a new alias

  intro:
    line1: Choose a name and email prefix for your new alias.

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
