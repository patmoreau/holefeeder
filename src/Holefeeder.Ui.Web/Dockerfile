﻿###################################
# Restore stage
###################################
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /source

COPY *.sln .
COPY .editorconfig .
COPY global.json .
COPY Directory.Build.props .
COPY Directory.Build.targets .
COPY Directory.Packages.props .
COPY nuget.config .
COPY src/Directory.Build.props src/

COPY src/Holefeeder.Ui.Shared/*.csproj src/Holefeeder.Ui.Shared/
COPY src/Holefeeder.Ui.Web/*.csproj src/Holefeeder.Ui.Web/

RUN dotnet restore src/Holefeeder.Ui.Web/Holefeeder.Ui.Web.csproj

###################################
# Build stage
###################################
FROM restore AS build
ARG BUILD_VERSION=99.99.99
ARG VERSION=${BUILD_VERSION:-99.99.99}
WORKDIR /source
COPY src/Holefeeder.Ui.Shared/. src/Holefeeder.Ui.Shared/.
COPY src/Holefeeder.Ui.Web/. src/Holefeeder.Ui.Web/.
WORKDIR /source/src/Holefeeder.Ui.Web

RUN dotnet publish /property:Version=${VERSION} /property:UseAppHost=false -c Release -o /app --no-restore

###################################
# Final stage
###################################
FROM nginx:alpine AS final
LABEL author="Drifter Apps Inc."
EXPOSE 8080

WORKDIR /usr/share/nginx/html
COPY --from=build /app/wwwroot .
