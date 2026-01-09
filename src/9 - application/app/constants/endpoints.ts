import { defineEndpoint } from "~/lib/endpoint";

export const endpoints = {
  AUTH: {
    TOKEN_EXCHANGE: defineEndpoint("/v1/tokens/exchange"),
    REQUEST_ACCESS: defineEndpoint("/v1/tokens/access"),
  },
} as const;
