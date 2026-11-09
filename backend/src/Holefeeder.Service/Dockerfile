###################################
# Restore stage
###################################
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /source

COPY *.slnx .
COPY .editorconfig .
COPY global.json .
COPY Directory.Build.props .
COPY Directory.Build.targets .
COPY Directory.Packages.props .
COPY nuget.config .
COPY src/Directory.Build.props src/

COPY src/Holefeeder.Service/*.csproj src/Holefeeder.Service/
COPY src/Holefeeder.Application/*.csproj src/Holefeeder.Application/
COPY src/Holefeeder.Domain/*.csproj src/Holefeeder.Domain/
COPY src/Holefeeder.Infrastructure/*.csproj src/Holefeeder.Infrastructure/

RUN dotnet restore src/Holefeeder.Service/Holefeeder.Service.csproj

###################################
# Build stage
###################################
FROM restore AS build
ARG BUILD_VERSION=99.99.99
ARG VERSION=${BUILD_VERSION:-99.99.99}
WORKDIR /source
COPY src/Holefeeder.Service/. src/Holefeeder.Service/.
COPY src/Holefeeder.Application/. src/Holefeeder.Application/.
COPY src/Holefeeder.Domain/. src/Holefeeder.Domain/.
COPY src/Holefeeder.Infrastructure/. src/Holefeeder.Infrastructure/.
WORKDIR /source/src/Holefeeder.Service

RUN dotnet publish /property:Version=${VERSION} -c Release -o /app --no-restore

###################################
# Final stage
###################################
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
LABEL author="Drifter Apps Inc."
EXPOSE 80
EXPOSE 443

WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Holefeeder.Service.dll"]
