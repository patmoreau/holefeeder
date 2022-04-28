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
WORKDIR /src
COPY src/. src/.
WORKDIR /src/src/Api/Holefeeder.Api
RUN dotnet publish /property:Version=${VERSION} --no-restore -c Release -o /app

FROM build as unittests
WORKDIR /src/src/Api/Holefeeder.UnitTests

FROM build as functionaltests
WORKDIR /src/src/Api/Holefeeder.FunctionalTests

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Holefeeder.Api.dll"]
