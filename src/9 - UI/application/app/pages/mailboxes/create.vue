<script setup lang="ts">
import { toast } from "vue-sonner";
import { toTypedSchema } from "@vee-validate/zod";
import { useForm } from "vee-validate";
import * as z from "zod";
import { Switch } from "@/components/ui/switch";

const { t } = useI18n({
  useScope: "local",
});

const { user } = useUserSession();
const { create } = useMailboxes();
const { refreshMailboxes } = useGlobalMailboxes();

const formSchema = toTypedSchema(
  z.object({
    name: z.string().trim().min(2, t("validation.name_min")),
    forwarding_email: z.string().trim().email(t("validation.forwarding_email")),
    auto_create_alias: z.boolean().default(true),
  })
);

const { handleSubmit } = useForm({
  validationSchema: formSchema,
  initialValues: {
    forwarding_email: user.value.email,
  },
});

const saving = ref(false);

const onSubmit = handleSubmit(async (formValues) => {
  saving.value = true;

  try {
    const mailbox = await create(
      "v1",
      {
        name: formValues.name,
        autoCreateAlias: formValues.auto_create_alias,
        emailAddress: formValues.forwarding_email,
      },
      {
        onResponse: async (ctx) => {
          toast.success(t("submit.success"));

          await refreshMailboxes().catch(() => {});
          await navigateTo(
            `/mailboxes/${ctx.response._data.value}/getting-started`
          );
        },
        onResponseError: async (ctx) => {
          toast.error(ctx?.response?.data?.title ?? t("submit.error"));
          saving.value = false;
          return;
        },
      }
    );
  } catch (error) {
  } finally {
    saving.value = false;
  }
});
</script>

<template>
  <div>
    <page-heading :title="t('page.title')" />

    <div class="mt-6 max-w-3xl">
      <form class="space-y-6" @submit="onSubmit">
        <FormField v-slot="{ componentField }" name="name">
          <FormItem>
            <FormLabel>{{ t("fields.name.label") }}</FormLabel>
            <FormControl>
              <Input
                type="text"
                v-bind="componentField"
                autofocus
                autocomplete="off"
              />
            </FormControl>
            <FormDescription>
              {{ t("fields.name.description") }}
            </FormDescription>
            <FormMessage />
          </FormItem>
        </FormField>

        <FormField v-slot="{ componentField }" name="forwarding_email">
          <FormItem>
            <FormLabel>{{ t("fields.forwarding_email.label") }}</FormLabel>
            <FormControl>
              <Input type="text" v-bind="componentField" disabled />
            </FormControl>
            <FormDescription>
              {{ t("fields.forwarding_email.description") }}
            </FormDescription>
            <FormMessage />
          </FormItem>
        </FormField>

        <FormField v-slot="{ value, handleChange }" name="auto_create_alias">
          <FormItem
            class="flex flex-row items-center justify-between rounded-lg border p-4"
          >
            <div class="space-y-0.5">
              <FormLabel class="text-base">
                {{ t("fields.auto_create_alias.label") }}
              </FormLabel>

              <FormDescription>
                {{
                  value
                    ? t("fields.auto_create_alias.description_on")
                    : t("fields.auto_create_alias.description_off")
                }}
              </FormDescription>
            </div>

            <FormControl>
              <Switch
                :model-value="value"
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
          />
          <div>
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
    title: Create mailbox

  intro:
    line1: "This is your private mailbox domain."

  coming_soon:
    title: Coming soon
    description: These features are not yet available.

  fields:
    name:
      label: Name
      description: The name of the mailbox.
    forwarding_email:
      label: Forwarding address
      description: Where we’ll forward incoming mail for this mailbox.
    auto_create_alias:
      label: Automatically create new aliases
      description_on: If mail arrives for an alias that doesn’t exist yet, we’ll create it automatically.
      description_off: Only existing aliases will receive mail. Create aliases before using them.

  validation:
    name_min: Name must be at least 2 characters long
    forwarding_email: Must be a valid email address

  submit:
    label: Create mailbox
    success: Your mailbox has been created successfully.
</i18n>
