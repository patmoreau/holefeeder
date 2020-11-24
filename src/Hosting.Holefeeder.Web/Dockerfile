FROM mcr.microsoft.com/dotnet/aspnet:3.1-buster-slim AS base
LABEL author="Drifter Apps Inc."
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1-buster AS build
WORKDIR /src

COPY DrifterApps.Holefeeder.sln ./
COPY Directory.Build.props ./

COPY Hosting.Gateway.API/*.csproj ./Hosting.Gateway.API/
COPY Hosting.Holefeeder.Web/*.csproj ./Hosting.Holefeeder.Web/
COPY Hosting.SeedWork/*.csproj ./Hosting.SeedWork/
COPY Hosting.Transactions.API/*.csproj ./Hosting.Transactions.API/
COPY Infrastructure.Database/*.csproj ./Infrastructure.Database/
COPY Application.Accounts/*.csproj ./Application.Accounts/
COPY Application.SeedWork/*.csproj ./Application.SeedWork/
COPY Application.Transactions/*.csproj ./Application.Transactions/
COPY Domain/*.csproj ./Domain/
COPY Framework.SeedWork/*.csproj ./Framework.SeedWork/

COPY tests/Directory.Build.props ./tests/
COPY tests/Application.Tests/*.csproj ./tests/Application.Tests/
COPY tests/Domain.Tests/*.csproj ./tests/Domain.Tests/
COPY tests/Infrastructure.Database.IntegrationTests/*.csproj ./tests/Infrastructure.Database.IntegrationTests/
COPY tests/Infrastructure.Database.Tests/*.csproj ./tests/Infrastructure.Database.Tests/

COPY DrifterApps.Holefeeder.Business/*.csproj ./DrifterApps.Holefeeder.Business/
COPY DrifterApps.Holefeeder.Business.Contracts/*.csproj ./DrifterApps.Holefeeder.Business.Contracts/
COPY DrifterApps.Holefeeder.Business.Entities/*.csproj ./DrifterApps.Holefeeder.Business.Entities/
COPY DrifterApps.Holefeeder.Common/*.csproj ./DrifterApps.Holefeeder.Common/
COPY DrifterApps.Holefeeder.Common.IoC/*.csproj ./DrifterApps.Holefeeder.Common.IoC/
COPY DrifterApps.Holefeeder.ResourcesAccess.Contracts/*.csproj ./DrifterApps.Holefeeder.ResourcesAccess.Contracts/
COPY DrifterApps.Holefeeder.ResourcesAccess.Mongo/*.csproj ./DrifterApps.Holefeeder.ResourcesAccess.Mongo/
COPY DrifterApps.Holefeeder.Services.BudgetApi/*.csproj ./DrifterApps.Holefeeder.Services.BudgetApi/
COPY DrifterApps.Holefeeder.Services.BudgetDto/*.csproj ./DrifterApps.Holefeeder.Services.BudgetDto/
COPY DrifterApps.Holefeeder.ServicesHosts.BudgetApi/*.csproj ./DrifterApps.Holefeeder.ServicesHosts.BudgetApi/

RUN dotnet restore DrifterApps.Holefeeder.sln

### for caching purposes, everything above this line should be the same in all dockerfiles of the solution

COPY . .

RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_15.x | bash \
    && apt-get install nodejs -yq \
    && npm install -g npm \
    && npm install -g @angular/cli

WORKDIR /src/Hosting.Holefeeder.Web/
RUN dotnet build --no-restore --configuration Release --output /app

FROM build AS testrunner
WORKDIR /src
RUN dotnet tool install dotnet-reportgenerator-globaltool --tool-path /dotnetglobaltools
LABEL testrunnerlayer=true
RUN dotnet test --no-restore --results-directory /out/testresults --collect:"XPlat Code Coverage" ./tests/Application.Tests/Application.Tests.csproj
RUN dotnet test --no-restore --results-directory /out/testresults --collect:"XPlat Code Coverage" ./tests/Domain.Tests/Domain.Tests.csproj
RUN dotnet test --no-restore --results-directory /out/testresults --collect:"XPlat Code Coverage" ./tests/Infrastructure.Database.Tests/Infrastructure.Database.Tests.csproj
RUN /dotnetglobaltools/reportgenerator "-reports:/out/testresults/*/coverage.cobertura.xml" "-targetdir:/out/coverage/reports" "-reporttypes:HTMLInline;Badges"
 
FROM testrunner AS publish
WORKDIR /src/Hosting.Holefeeder.Web/
RUN dotnet publish --no-restore --configuration Release --output /app

FROM base AS final

RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_15.x | bash \
    && apt-get install nodejs -yq

WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Hosting.Holefeeder.Web.dll"]
