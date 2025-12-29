# Build Stage 1
FROM node:22-bookworm-slim AS build
WORKDIR /app

# Copy manifests first (cache-friendly)
COPY package.json package-lock.json .npmrc* ./

# Install dependencies
RUN npm ci --no-audit --no-fund

# Copy the entire project
COPY . ./

# Build the project
RUN NODE_OPTIONS="--max-old-space-size=8192" npm run build

# Build Stage 2
FROM node:22-bookworm-slim
WORKDIR /app

ENV NODE_ENV=production
ENV HOST=0.0.0.0
ENV PORT=80

# Only `.output` is needed from the build stage
COPY --from=build /app/.output/ ./

EXPOSE 80

CMD ["node", "/app/server/index.mjs"]
