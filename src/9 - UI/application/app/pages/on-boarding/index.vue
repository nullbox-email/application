<script setup lang="ts">
import { toast } from "vue-sonner";
import { toTypedSchema } from "@vee-validate/zod";
import { useForm } from "vee-validate";
import * as z from "zod";

definePageMeta({
  layout: "full",
});

useHead({
  title: "Welcome",
});

const { t } = useI18n({
  useScope: "local",
});

const schema = toTypedSchema(
  z.object({
    name: z
      .string({ required_error: t("name.validation.required") })
      .min(3)
      .max(64)
      .trim(),
  })
);

const token = ref<string | null>(null);

const { handleSubmit } = useForm({
  validationSchema: schema,
});

const saving = ref(false);

// number of quote options in i18n
const QUOTE_COUNT = 14;

// SSR-persisted so hydration doesn't swap the quote
const quoteIndex = useState<number>("onboarding-quote-index", () =>
  Math.floor(Math.random() * QUOTE_COUNT)
);

const selectedQuote = computed(() => t(`quote.options.${quoteIndex.value}`));

const onSubmit = handleSubmit(async (values) => {
  saving.value = true;

  const { createUserProfile } = useOnboarding();

  try {
    await createUserProfile(
      "v1",
      {
        name: values.name,
        cfTurnstileResponse: token.value ?? "",
      },
      {
        onResponse: async () => {
          toast.success(t("submit.success"));
          await navigateTo("/on-boarding/finalize", { replace: true });
        },
        onResponseError: async (ctx) => {
          toast.error(ctx?.response?._data ?? t("submit.error"));
          saving.value = false;
          return;
        },
      }
    );
  } catch (_error: any) {
  } finally {
    saving.value = false;
  }
});

const buttonLabel = computed(() =>
  token.value ? t("submit.label") : t("submit.validating")
);
</script>

<template>
  <div class="grid min-h-svh lg:grid-cols-2">
    <div
      class="relative hidden h-full flex-col bg-muted text-white dark:border-r lg:flex"
    >
      <nuxt-img
        src="/assets/background/focus.svg"
        :alt="t('hero.alt')"
        class="absolute inset-0 h-full w-full object-cover"
      />

      <!-- Particle overlay -->
      <ParticleRain
        class="absolute inset-0"
        :count="100"
        :opacity="0.22"
        :speed="1.05"
        :sprite-scale="1.35"
        :particle-r-g-b="[255, 255, 255]"
      />

      <div class="absolute z-20 bottom-10 mx-10">
        <blockquote class="space-y-2">
          <p class="text-2xl">{{ selectedQuote }}</p>
        </blockquote>
      </div>
    </div>

    <div class="flex flex-col gap-4 p-6 md:p-10">
      <div class="flex justify-center gap-2 md:justify-end">
        <div class="flex items-center gap-2 text-2xl font-medium">
          <img
            src="/assets/logo.light.svg"
            alt="Nullbox Logo"
            class="block h-16 w-auto dark:hidden"
          />
          <img
            src="/assets/logo.dark.svg"
            alt="Nullbox Logo"
            class="hidden h-16 w-auto dark:block"
          />
        </div>
      </div>

      <div class="flex flex-1 items-center justify-center">
        <div class="lg:p-8">
          <div
            class="mx-auto flex w-full flex-col justify-center space-y-6 sm:w-[350px]"
          >
            <div class="flex flex-col space-y-2 text-center">
              <h1 class="text-2xl font-semibold tracking-tight">
                {{ t("title") }}
              </h1>
              <p class="text-sm text-muted-foreground">
                {{ t("description") }}
              </p>
            </div>

            <form @submit.prevent="onSubmit" class="grid gap-4">
              <FormField v-slot="{ componentField }" name="name">
                <FormItem>
                  <FormControl>
                    <Input
                      autofocus
                      autocomplete="name"
                      type="text"
                      :placeholder="t('name.placeholder')"
                      v-bind="componentField"
                      :disabled="saving"
                      @keyup.enter="onSubmit"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              </FormField>

              <NuxtTurnstile v-model="token" class="h-0" />

              <Button
                class="cursor-pointer"
                type="submit"
                :disabled="saving || !token"
              >
                <div
                  v-if="!token"
                  class="flex items-center space-x-2"
                >
                  <icon name="lucide:loader-circle" class="animate-spin" />
                  <span> {{ buttonLabel }} </span>
                </div>
                <div v-else>
                  <icon
                    v-if="saving"
                    name="lucide:loader-circle"
                    class="animate-spin"
                  />
                  <div v-else>
                    {{ buttonLabel }}
                  </div>
                </div>
              </Button>
            </form>

            <p class="px-8 text-center text-sm text-muted-foreground">
              {{ t("legal.notice") }}
            </p>
            <div
              class="mt-2 flex justify-center gap-4 text-xs text-muted-foreground"
            >
              <nuxt-link
                to="/terms"
                class="underline underline-offset-4 hover:text-primary"
              >
                {{ t("legal.terms") }}
              </nuxt-link>
              <nuxt-link
                to="/privacy"
                class="underline underline-offset-4 hover:text-primary"
              >
                {{ t("legal.privacy") }}
              </nuxt-link>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  title: Welcome
  description: Let’s start with your name.
  hero:
    alt: Focus background image
  quote:
    options:
      - Privacy isn’t a feature. It’s the foundation.
      - Your information belongs to you. We keep it that way.
      - Start privately. Stay private.
      - Strong privacy, minimal friction.
      - Stay reachable without being exposed.
      - Give out an email, not your identity.
      - Your inbox should answer to you, not advertisers.
      - Keep your real address off the front lines.
      - Control what reaches you, and what doesn’t.
      - Less tracking. Same email.
      - Block the data grab, keep the messages.
      - A safer inbox starts with a little distance.
      - Good tools protect you, even when you’re not looking.
      - Privacy is freedom in the digital world.
  name:
    placeholder: Your name
    validation:
      required: Please tell us what to call you.
  submit:
    label: Next
    validating: Proving you’re human...
    success: Your profile has been created successfully.
    error: Something went wrong while creating your profile.
  legal:
    notice: By clicking next, you agree to our Terms of Service and Privacy Policy.
    terms: Terms of Service
    privacy: Privacy Policy
</i18n>
