###################################
# Restore stage
###################################
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /source

COPY *.sln .
COPY .editorconfig .
COPY global.json .
COPY Directory.Build.props .
COPY Directory.Build.targets .
COPY Directory.Packages.props .
COPY nuget.config .
COPY src/Directory.Build.props src/
COPY tests/.editorconfig tests/
COPY tests/coverlet.runsettings tests/
COPY tests/Directory.Build.props tests/
COPY tests/Directory.Packages.props tests/

COPY src/Holefeeder.Api/*.csproj src/Holefeeder.Api/
COPY src/Holefeeder.Application/*.csproj src/Holefeeder.Application/
COPY src/Holefeeder.Domain/*.csproj src/Holefeeder.Domain/
COPY src/Holefeeder.Infrastructure/*.csproj src/Holefeeder.Infrastructure/

COPY tests/Holefeeder.UnitTests/*.csproj tests/Holefeeder.UnitTests/
COPY tests/Holefeeder.Tests.Common/*.csproj tests/Holefeeder.Tests.Common/
COPY tests/Holefeeder.FunctionalTests/*.csproj tests/Holefeeder.FunctionalTests/

RUN dotnet restore src/Holefeeder.Api/Holefeeder.Api.csproj

RUN dotnet restore tests/Holefeeder.UnitTests/Holefeeder.UnitTests.csproj
RUN dotnet restore tests/Holefeeder.Tests.Common/Holefeeder.Tests.Common.csproj
RUN dotnet restore tests/Holefeeder.FunctionalTests/Holefeeder.FunctionalTests.csproj

###################################
# Build stage
###################################
FROM restore AS build
ARG BUILD_VERSION=99.99.99
ARG VERSION=${BUILD_VERSION:-99.99.99}
WORKDIR /source
COPY src/Holefeeder.Api/. src/Holefeeder.Api/.
COPY src/Holefeeder.Application/. src/Holefeeder.Application/.
COPY src/Holefeeder.Domain/. src/Holefeeder.Domain/.
COPY src/Holefeeder.Infrastructure/. src/Holefeeder.Infrastructure/.
COPY tests/Holefeeder.UnitTests/. tests/Holefeeder.UnitTests/.
COPY tests/Holefeeder.Tests.Common/. tests/Holefeeder.Tests.Common/.
COPY tests/Holefeeder.FunctionalTests/. tests/Holefeeder.FunctionalTests/.
WORKDIR /source/src/Holefeeder.Api

RUN dotnet publish /property:Version=${VERSION} -c Release -o /app --no-restore

###################################
# Unit tests stage
###################################
FROM build AS unittests
WORKDIR /source/tests/Holefeeder.UnitTests

###################################
# Functional tests stage
###################################
FROM build AS functionaltests
WORKDIR /source/tests/Holefeeder.FunctionalTests

RUN dotnet dev-certs https --trust

###################################
# Final stage
###################################
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
LABEL author="Drifter Apps Inc."
EXPOSE 80
EXPOSE 443

WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Holefeeder.Api.dll"]
