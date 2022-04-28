FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
LABEL author="Drifter Apps Inc."
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS restore
WORKDIR /src

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY DrifterApps.Holefeeder.sln ./
COPY Directory.Build.props ./

COPY src/Api/Holefeeder.Api/*.csproj src/Api/Holefeeder.Api/
COPY src/Api/Holefeeder.Application/*.csproj src/Api/Holefeeder.Application/
COPY src/Api/Holefeeder.Domain/*.csproj src/Api/Holefeeder.Domain/
COPY src/Api/Holefeeder.Infrastructure/*.csproj src/Api/Holefeeder.Infrastructure/
COPY src/Web/Holefeeder.Web/*.csproj src/Web/Holefeeder.Web/

COPY src/Api/Holefeeder.UnitTests/*.csproj src/Api/Holefeeder.UnitTests/
COPY src/Api/Holefeeder.Tests.Common/*.csproj src/Api/Holefeeder.Tests.Common/
COPY src/Api/Holefeeder.FunctionalTests/*.csproj src/Api/Holefeeder.FunctionalTests/

RUN dotnet restore DrifterApps.Holefeeder.sln

FROM restore as build
ARG BUILD_VERSION=99.99.99
ARG VERSION=${BUILD_VERSION:-99.99.99}

# Install Node.js, yarn and build-essential
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash
RUN curl https://dl.yarnpkg.com/debian/pubkey.gpg | apt-key add -
RUN echo "deb https://dl.yarnpkg.com/debian/ stable main" | tee /etc/apt/sources.list.d/yarn.list
RUN apt-get update && apt-get install -y nodejs yarn build-essential

WORKDIR /src
COPY src/. src/.
WORKDIR /src/src/Web/Holefeeder.Web
RUN dotnet build /property:Version=${VERSION} --no-restore -c Release

FROM build AS publish
WORKDIR /src/src/Web/Holefeeder.Web
RUN dotnet publish /property:Version=${VERSION} --no-restore --no-build -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Holefeeder.Web.dll"]
