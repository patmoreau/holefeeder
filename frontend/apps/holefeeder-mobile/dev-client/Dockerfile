FROM node:22-slim AS base

# Install pnpm and build dependencies
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
RUN corepack enable

RUN apt-get update && apt-get install -y \
    python3 \
    make \
    g++ \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy lockfile and package.json first for layer caching
COPY dev-client/package.json ./

# Install dependencies
RUN pnpm install --shamefully-hoist

# Rebuild all native modules for the container architecture
# Need to rebuild the nested @powersync/better-sqlite3 package
RUN cd /app/node_modules/.pnpm/@powersync+better-sqlite3@*/node_modules/@powersync/better-sqlite3 && pnpm rebuild || true
RUN cd /app/node_modules/.pnpm/better-sqlite3@*/node_modules/better-sqlite3 && pnpm rebuild || true

# Copy the dev-client source files (needed before rebuild)
COPY dev-client/*.ts ./
COPY dev-client/tsconfig.json ./
COPY src/domain/persistence/app-schema.ts ./

# Ensure data directory exists
RUN mkdir -p /data

CMD ["pnpm", "start"]