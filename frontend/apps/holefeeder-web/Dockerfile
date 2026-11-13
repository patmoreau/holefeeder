###################################
# Build stage
###################################
# Built from the frontend monorepo; the image builds from frontend/ so the shared
# package is in context (see frontend/.dockerignore).
FROM node:24-slim AS build
WORKDIR /frontend
RUN corepack enable

COPY package.json pnpm-lock.yaml pnpm-workspace.yaml tsconfig.json tsconfig.base.json ./
COPY packages/shared/package.json packages/shared/
COPY apps/holefeeder-web/package.json apps/holefeeder-web/
COPY apps/holefeeder-mobile/package.json apps/holefeeder-mobile/
RUN pnpm install --frozen-lockfile --filter "holefeeder-web..."

COPY packages/shared/tsconfig.json packages/shared/
COPY packages/shared/src packages/shared/src
COPY apps/holefeeder-web/index.html apps/holefeeder-web/tsconfig.json apps/holefeeder-web/tsconfig.node.json apps/holefeeder-web/vite.config.ts apps/holefeeder-web/
COPY apps/holefeeder-web/public apps/holefeeder-web/public
COPY apps/holefeeder-web/src apps/holefeeder-web/src

ARG VITE_AUTH0_DOMAIN=dev-vx1jio3owhaqdmqa.ca.auth0.com
ARG VITE_AUTH0_CLIENT_ID=gXVZOUw6cxoIR6N5qqXFyRVuZDbkkVxu
ARG VITE_AUTH0_AUDIENCE=https://holefeeder-api.drifterapps.app
ARG VITE_API_BASE_URL=/gateway
ARG BUILD_VERSION=
ARG GIT_COMMIT=
ENV VITE_AUTH0_DOMAIN=${VITE_AUTH0_DOMAIN} \
    VITE_AUTH0_CLIENT_ID=${VITE_AUTH0_CLIENT_ID} \
    VITE_AUTH0_AUDIENCE=${VITE_AUTH0_AUDIENCE} \
    VITE_API_BASE_URL=${VITE_API_BASE_URL} \
    VITE_APP_VERSION=${BUILD_VERSION} \
    VITE_APP_COMMIT=${GIT_COMMIT}
RUN pnpm --filter "holefeeder-web..." run build

###################################
# Final stage
###################################
FROM nginx:1.29-alpine AS final
LABEL author="Drifter Apps Inc."
EXPOSE 80

COPY apps/holefeeder-web/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /frontend/apps/holefeeder-web/dist /usr/share/nginx/html
