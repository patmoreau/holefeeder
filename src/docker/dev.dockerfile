##### Stage 1
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
LABEL author="Drifter Apps Inc."
WORKDIR /app

# copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

# copy everything else and build app
RUN dotnet publish ./DrifterApps.Holefeeder.ServicesHosts.API/DrifterApps.Holefeeder.ServicesHosts.API.csproj -c Debug -o /app/work

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime
WORKDIR /app
EXPOSE 5001
COPY --from=build /app/work .
ENTRYPOINT ["dotnet", "DrifterApps.Holefeeder.ServicesHosts.API.dll"]

# docker build -t holefeeder-api -f docker/dev.dockerfile .

# docker run -d -p 8080:80 holefeeder-web
# docker tag holefeeder-web:latest registry.gitlab.com/drifterapps/holefeeder-web:1.0.0
# docker push registry.gitlab.com/drifterapps/holefeeder-web
# docker login YN6hzx73GnzmDzTYfnnq
# docker run -d -e DATABASE_URL='mongodb://service:X9ukI8DRu6Fz@ds036967.mlab.com:36967/holefeeder' -e DATABASE_NAME='holefeeder' -e ASPNETCORE_ENVIRONMENT='Development' -p 5001:80 holefeeder-api
# docker run -d -e DATABASE_URL='mongodb://localhost:27017/holefeeder' -e DATABASE_NAME='holefeeder' -e ASPNETCORE_ENVIRONMENT='Development' -p 5001:80 holefeeder-api
