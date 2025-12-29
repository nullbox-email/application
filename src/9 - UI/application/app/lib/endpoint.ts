export type Primitive = string | number | boolean;
export type QueryParams = Record<string, Primitive | null | undefined>;

// Infer `:param` keys from a template like "/v1/media/:containerId/:path"
type ParamName<S extends string> =
  S extends `${string}:${infer P}/${infer Rest}` ? P | ParamName<`/${Rest}`>
  : S extends `${string}:${infer P}` ? P
  : never;

export type PathParams<S extends string> =
  [ParamName<S>] extends [never] ? Record<string, never> : { [K in ParamName<S>]: string | number };

export interface Endpoint<Path extends string> {
  path: Path;
  build: (params?: PathParams<Path>, query?: QueryParams) => string;
  toString(): Path;
}

export function defineEndpoint<P extends string>(path: P): Endpoint<P> {
  function build(params?: PathParams<P>, query?: QueryParams): string {
    let url = path as string;

    if (params) {
      for (const [k, v] of Object.entries(params)) {
        url = url.replace(`:${k}`, encodeURIComponent(String(v)));
      }
    }

    if (query) {
      const qs = Object.entries(query)
        .filter(([, v]) => v !== null && v !== undefined)
        .map(([k, v]) => `${encodeURIComponent(k)}=${encodeURIComponent(String(v))}`)
        .join("&");
      if (qs) url += `?${qs}`;
    }
    return url;
  }

  return {
    path,
    build,
    toString: () => path,
  } as const;
}
