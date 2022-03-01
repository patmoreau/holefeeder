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

COPY src/ApiGateways/Web.Gateway/*.csproj src/ApiGateways/Web.Gateway/
COPY src/BuildingBlocks/Framework.Dapper.SeedWork/*.csproj src/BuildingBlocks/Framework.Dapper.SeedWork/
COPY src/BuildingBlocks/Framework.Mongo.SeedWork/*.csproj src/BuildingBlocks/Framework.Mongo.SeedWork/
COPY src/BuildingBlocks/Framework.SeedWork/*.csproj src/BuildingBlocks/Framework.SeedWork/
COPY src/Services/Budgeting/Budgeting.API/*.csproj src/Services/Budgeting/Budgeting.API/
COPY src/Services/Budgeting/Budgeting.Application/*.csproj src/Services/Budgeting/Budgeting.Application/
COPY src/Services/Budgeting/Budgeting.Domain/*.csproj src/Services/Budgeting/Budgeting.Domain/
COPY src/Services/Budgeting/Budgeting.Infrastructure/*.csproj src/Services/Budgeting/Budgeting.Infrastructure/
COPY src/Services/ObjectStore/ObjectStore.API/*.csproj src/Services/ObjectStore/ObjectStore.API/
COPY src/Services/ObjectStore/ObjectStore.Application/*.csproj src/Services/ObjectStore/ObjectStore.Application/
COPY src/Services/ObjectStore/ObjectStore.Domain/*.csproj src/Services/ObjectStore/ObjectStore.Domain/
COPY src/Services/ObjectStore/ObjectStore.Infrastructure/*.csproj src/Services/ObjectStore/ObjectStore.Infrastructure/
COPY src/Web/Holefeeder.Web/*.csproj src/Web/Holefeeder.Web/

COPY src/Services/Budgeting/Budgeting.UnitTests/*.csproj src/Services/Budgeting/Budgeting.UnitTests/
COPY src/Services/Budgeting/Budgeting.FunctionalTests/*.csproj src/Services/Budgeting/Budgeting.FunctionalTests/
COPY src/Services/ObjectStore/ObjectStore.UnitTests/*.csproj src/Services/ObjectStore/ObjectStore.UnitTests/
COPY src/Services/ObjectStore/ObjectStore.FunctionalTests/*.csproj src/Services/ObjectStore/ObjectStore.FunctionalTests/

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
