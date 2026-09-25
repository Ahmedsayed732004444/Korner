# syntax=docker/dockerfile:1
FROM node:24-alpine AS base
WORKDIR /app
RUN corepack enable

FROM base AS deps
# scripts/ is copied here too: the "prepare" lifecycle script runs on every
# `pnpm install`, and it must exist even in this deps-only layer.
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./
COPY scripts ./scripts
RUN pnpm install --frozen-lockfile

FROM deps AS build
COPY . .
RUN pnpm build

FROM base AS production-deps
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./
COPY scripts ./scripts
RUN pnpm install --frozen-lockfile --prod

FROM base AS final
ENV NODE_ENV=production
COPY --from=production-deps /app/node_modules ./node_modules
COPY --from=build /app/build ./build
COPY package.json ./

USER node
EXPOSE 3000
ENV PORT=3000
# Invoke the installed binary directly: `pnpm start` would make corepack lazily
# fetch the pnpm binary at container startup, which fails as the non-root user.
CMD ["node_modules/.bin/react-router-serve", "./build/server/index.js"]
