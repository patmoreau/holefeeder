###################################
# Node and pnpm install stage
###################################
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS pre-build
WORKDIR /source

# Download and import the Nodesource GPG key
RUN apt-get update
RUN apt-get install -y ca-certificates curl gnupg
RUN mkdir -p /etc/apt/keyrings
RUN curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg

# Create deb repository
ENV NODE_MAJOR=22
RUN echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_$NODE_MAJOR.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list

# Run Update and Install in a single RUN command to ensure freshness of package lists
# Adding a retry mechanism with apt-get install
RUN apt-get update && \
    apt-get install -y --no-install-recommends nodejs build-essential && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Install pnpm
RUN npm install -g pnpm

# Allow lifecycle/build scripts in CI/container builds
ENV PNPM_ALLOW_UNTRUSTED_SCRIPTS=true
ENV CI=true
RUN pnpm config set ignore-scripts false
RUN pnpm config set dangerouslyAllowAllBuilds true

###################################
# Restore stage
###################################
FROM pre-build AS restore

COPY backend/*.slnx .
COPY backend/.editorconfig .
COPY backend/global.json .
COPY backend/Directory.Build.props .
COPY backend/Directory.Build.targets .
COPY backend/Directory.Packages.props .
COPY backend/nuget.config .
COPY backend/src/Directory.Build.props src/

COPY backend/src/Holefeeder.Web/*.csproj src/Holefeeder.Web/

RUN dotnet restore src/Holefeeder.Web/Holefeeder.Web.csproj

###################################
# Build stage
###################################
FROM restore AS build
ARG BUILD_VERSION=99.99.99
ARG VERSION=${BUILD_VERSION:-99.99.99}

WORKDIR /source
COPY backend/src/Holefeeder.Web/. src/Holefeeder.Web/.
WORKDIR /source/src/Holefeeder.Web

RUN dotnet publish /property:Version=${VERSION} -c Release -o /app --no-restore

###################################
# Unit tests stage
###################################
FROM build AS angulartests
WORKDIR /source/src/Holefeeder.Web/ClientApp

RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN sh -c 'echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list'
RUN apt-get update
RUN apt --fix-broken install
RUN apt-get install google-chrome-stable -y

###################################
# React web app stage
###################################
# Built from the frontend monorepo; the image builds from the repository root
# so both backend/ and frontend/ are in context (see /.dockerignore).
FROM node:24-slim AS react
WORKDIR /frontend
RUN corepack enable

COPY frontend/package.json frontend/pnpm-lock.yaml frontend/pnpm-workspace.yaml frontend/tsconfig.json ./
COPY frontend/packages/shared/package.json packages/shared/
COPY frontend/apps/holefeeder-web/package.json apps/holefeeder-web/
COPY frontend/apps/holefeeder-mobile/package.json apps/holefeeder-mobile/
RUN pnpm install --frozen-lockfile --filter "holefeeder-web..."

COPY frontend/packages/shared/tsconfig.json packages/shared/
COPY frontend/packages/shared/src packages/shared/src
COPY frontend/apps/holefeeder-web/index.html frontend/apps/holefeeder-web/tsconfig.json frontend/apps/holefeeder-web/tsconfig.node.json frontend/apps/holefeeder-web/vite.config.ts apps/holefeeder-web/
COPY frontend/apps/holefeeder-web/public apps/holefeeder-web/public
COPY frontend/apps/holefeeder-web/src apps/holefeeder-web/src

ARG VITE_AUTH0_DOMAIN=dev-vx1jio3owhaqdmqa.ca.auth0.com
ARG VITE_AUTH0_CLIENT_ID=gXVZOUw6cxoIR6N5qqXFyRVuZDbkkVxu
ARG VITE_AUTH0_AUDIENCE=https://holefeeder-api.drifterapps.app
ARG VITE_API_BASE_URL=/gateway
ENV VITE_AUTH0_DOMAIN=${VITE_AUTH0_DOMAIN} \
    VITE_AUTH0_CLIENT_ID=${VITE_AUTH0_CLIENT_ID} \
    VITE_AUTH0_AUDIENCE=${VITE_AUTH0_AUDIENCE} \
    VITE_API_BASE_URL=${VITE_API_BASE_URL}
RUN pnpm --filter "holefeeder-web..." run build

###################################
# Final stage
###################################
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
LABEL author="Drifter Apps Inc."
EXPOSE 80
EXPOSE 443

WORKDIR /app
COPY --from=build /app ./
COPY --from=react /frontend/apps/holefeeder-web/dist ./wwwroot/
ENTRYPOINT ["dotnet", "Holefeeder.Web.dll"]
