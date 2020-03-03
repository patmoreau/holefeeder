##### Stage 1
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
LABEL author="Drifter Apps Inc."
WORKDIR /app/

# copy csproj and restore as distinct layers
COPY src/HoleFeederApi/*.csproj ./src/HoleFeederApi/
WORKDIR /app/src/HoleFeederApi/
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app/
COPY src/HoleFeederApi/. ./src/HoleFeederApi/
WORKDIR /app/src/HoleFeederApi/
RUN dotnet publish -c Release -o out

# test application -- see: dotnet-docker-unit-testing.md
FROM build AS testrunner
WORKDIR /app/
COPY tests/HoleFeederApi.Tests/. ./tests/HoleFeederApi.Tests/
WORKDIR /app/tests/HoleFeederApi.Tests/
RUN dotnet build
ENTRYPOINT ["sh", "/app/tests/HoleFeederApi.Tests/runtests.sh"]

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime
WORKDIR /app
EXPOSE 5001
COPY --from=build /app/src/HoleFeederApi/out ./
ENTRYPOINT ["dotnet", "HoleFeederApi.dll"]
