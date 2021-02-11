FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
LABEL author="Drifter Apps Inc."
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS restore
WORKDIR /src

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY DrifterApps.Holefeeder.sln ./
COPY Directory.Build.props ./

COPY ApiGateways/Web.Gateway/*.csproj ApiGateways/Web.Gateway/
COPY BuildingBlocks/Framework.Mongo.SeedWork/*.csproj BuildingBlocks/Framework.Mongo.SeedWork/
COPY BuildingBlocks/Framework.SeedWork/*.csproj BuildingBlocks/Framework.SeedWork/
COPY Services/Budgeting/Budgeting.API/*.csproj Services/Budgeting/Budgeting.API/
COPY Services/Budgeting/Budgeting.Application/*.csproj Services/Budgeting/Budgeting.Application/
COPY Services/Budgeting/Budgeting.Domain/*.csproj Services/Budgeting/Budgeting.Domain/
COPY Services/Budgeting/Budgeting.Infrastructure/*.csproj Services/Budgeting/Budgeting.Infrastructure/
COPY Services/ObjectStore/ObjectStore.API/*.csproj Services/ObjectStore/ObjectStore.API/
COPY Services/ObjectStore/ObjectStore.Application/*.csproj Services/ObjectStore/ObjectStore.Application/
COPY Services/ObjectStore/ObjectStore.Domain/*.csproj Services/ObjectStore/ObjectStore.Domain/
COPY Services/ObjectStore/ObjectStore.Infrastructure/*.csproj Services/ObjectStore/ObjectStore.Infrastructure/
COPY Web/Holefeeder.Web/*.csproj Web/Holefeeder.Web/

COPY Services/Budgeting/Budgeting.UnitTests/*.csproj Services/Budgeting/Budgeting.UnitTests/
COPY Services/Budgeting/Budgeting.FunctionalTests/*.csproj Services/Budgeting/Budgeting.FunctionalTests/
COPY Services/ObjectStore/ObjectStore.UnitTests/*.csproj Services/ObjectStore/ObjectStore.UnitTests/
COPY Services/ObjectStore/ObjectStore.FunctionalTests/*.csproj Services/ObjectStore/ObjectStore.FunctionalTests/

COPY Legacy/DrifterApps.Holefeeder.Business/*.csproj Legacy/DrifterApps.Holefeeder.Business/
COPY Legacy/DrifterApps.Holefeeder.Business.Contracts/*.csproj Legacy/DrifterApps.Holefeeder.Business.Contracts/
COPY Legacy/DrifterApps.Holefeeder.Business.Entities/*.csproj Legacy/DrifterApps.Holefeeder.Business.Entities/
COPY Legacy/DrifterApps.Holefeeder.Common/*.csproj Legacy/DrifterApps.Holefeeder.Common/
COPY Legacy/DrifterApps.Holefeeder.Common.IoC/*.csproj Legacy/DrifterApps.Holefeeder.Common.IoC/
COPY Legacy/DrifterApps.Holefeeder.ResourcesAccess.Contracts/*.csproj Legacy/DrifterApps.Holefeeder.ResourcesAccess.Contracts/
COPY Legacy/DrifterApps.Holefeeder.ResourcesAccess.Mongo/*.csproj Legacy/DrifterApps.Holefeeder.ResourcesAccess.Mongo/
COPY Legacy/DrifterApps.Holefeeder.Services.BudgetApi/*.csproj Legacy/DrifterApps.Holefeeder.Services.BudgetApi/
COPY Legacy/DrifterApps.Holefeeder.Services.BudgetDto/*.csproj Legacy/DrifterApps.Holefeeder.Services.BudgetDto/
COPY Legacy/DrifterApps.Holefeeder.ServicesHosts.BudgetApi/*.csproj Legacy/DrifterApps.Holefeeder.ServicesHosts.BudgetApi/

RUN dotnet restore DrifterApps.Holefeeder.sln

FROM restore as build
ARG holefeeder_version=99.99.99
WORKDIR /src
COPY . .
WORKDIR /src/Services/ObjectStore/ObjectStore.API
RUN dotnet publish /property:Version=${holefeeder_version} --no-restore -c Release -o /app

FROM build as unittest
WORKDIR /src/Services/ObjectStore/ObjectStore.UnitTests

FROM build as functionaltest
WORKDIR /src/Services/ObjectStore/ObjectStore.FunctionalTests

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ObjectStore.API.dll"]
