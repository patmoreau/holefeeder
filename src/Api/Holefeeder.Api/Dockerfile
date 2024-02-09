###################################
# Restore stage
###################################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /source

COPY *.sln .
COPY .editorconfig .
COPY global.json .
COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY nuget.config .

COPY src/Api/Holefeeder.Api/*.csproj src/Api/Holefeeder.Api/
COPY src/Api/Holefeeder.Application/*.csproj src/Api/Holefeeder.Application/
COPY src/Api/Holefeeder.Domain/*.csproj src/Api/Holefeeder.Domain/
COPY src/Api/Holefeeder.Infrastructure/*.csproj src/Api/Holefeeder.Infrastructure/
COPY src/Web/Holefeeder.Web/*.csproj src/Web/Holefeeder.Web/

COPY src/Api/Holefeeder.UnitTests/*.csproj src/Api/Holefeeder.UnitTests/
COPY src/Api/Holefeeder.Tests.Common/*.csproj src/Api/Holefeeder.Tests.Common/
COPY src/Api/Holefeeder.FunctionalTests/*.csproj src/Api/Holefeeder.FunctionalTests/

RUN dotnet restore

###################################
# Build stage
###################################
FROM restore as build
ARG BUILD_VERSION=99.99.99
ARG VERSION=${BUILD_VERSION:-99.99.99}
WORKDIR /source
COPY src/. src/.
WORKDIR /source/src/Api/Holefeeder.Api

RUN dotnet publish /property:Version=${VERSION} -c Release -o /app --no-restore

###################################
# Unit tests stage
###################################
FROM build as unittests
WORKDIR /source/src/Api/Holefeeder.UnitTests

###################################
# Functional tests stage
###################################
FROM build as functionaltests
WORKDIR /source/src/Api/Holefeeder.FunctionalTests

###################################
# Final stage
###################################
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as final
LABEL author="Drifter Apps Inc."
EXPOSE 80
EXPOSE 443

WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Holefeeder.Api.dll"]
