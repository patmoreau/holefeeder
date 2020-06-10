FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
LABEL author="Drifter Apps Inc."
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80
ENV ASPNETCORE_ENVIRONMENT="development"

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
COPY DrifterApps.Holefeeder.sln ./
COPY src/API.Accounts/*.csproj ./src/API.Accounts/
COPY src/API.OcelotGateway/*.csproj ./src/API.OcelotGateway/
COPY src/Infrastructure.MongoDB/*.csproj ./src/Infrastructure.MongoDB/
COPY src/Application.Accounts/*.csproj ./src/Application.Accounts/
COPY src/Domain/*.csproj ./src/Domain/
COPY tests/Infrastructure.MongoDB.Tests/*.csproj ./tests/Infrastructure.MongoDB.Tests/
COPY tests/Application.Tests/*.csproj ./tests/Application.Tests/
COPY tests/Domain.Tests/*.csproj ./tests/Domain.Tests/

RUN dotnet restore DrifterApps.Holefeeder.sln

COPY . ./
WORKDIR ./src/API.OcelotGateway/
RUN dotnet build --no-restore -c Debug -o /app

#FROM build as unittest
#WORKDIR /src/Services/Ordering/Ordering.UnitTests

#FROM build as functionaltest
#WORKDIR /src/Services/Ordering/Ordering.FunctionalTests

FROM build AS publish
RUN dotnet publish -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "API.OcelotGateway.dll"]
