FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine AS base
LABEL author="Drifter Apps Inc."
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine AS build
WORKDIR /src

COPY src/DrifterApps.Holefeeder.sln ./
COPY src/Directory.Build.props ./

COPY src/Hosting.Gateway.API/*.csproj ./Hosting.Gateway.API/
COPY src/Hosting.Holefeeder.Web/*.csproj ./Hosting.Holefeeder.Web/
COPY src/Hosting.SeedWork/*.csproj ./Hosting.SeedWork/
COPY src/Hosting.Transactions.API/*.csproj ./Hosting.Transactions.API/
COPY src/Infrastructure.Database/*.csproj ./Infrastructure.Database/
COPY src/Application.Accounts/*.csproj ./Application.Accounts/
COPY src/Application.SeedWork/*.csproj ./Application.SeedWork/
COPY src/Application.Transactions/*.csproj ./Application.Transactions/
COPY src/Domain/*.csproj ./Domain/
COPY src/Framework.SeedWork/*.csproj ./Framework.SeedWork/

COPY src/tests/Directory.Build.props ./tests/
COPY src/tests/Application.Tests/*.csproj ./tests/Application.Tests/
COPY src/tests/Domain.Tests/*.csproj ./tests/Domain.Tests/
COPY src/tests/Infrastructure.Database.IntegrationTests/*.csproj ./tests/Infrastructure.Database.IntegrationTests/
COPY src/tests/Infrastructure.Database.Tests/*.csproj ./tests/Infrastructure.Database.Tests/

COPY src/DrifterApps.Holefeeder.Business/*.csproj ./DrifterApps.Holefeeder.Business/
COPY src/DrifterApps.Holefeeder.Business.Contracts/*.csproj ./DrifterApps.Holefeeder.Business.Contracts/
COPY src/DrifterApps.Holefeeder.Business.Entities/*.csproj ./DrifterApps.Holefeeder.Business.Entities/
COPY src/DrifterApps.Holefeeder.Common/*.csproj ./DrifterApps.Holefeeder.Common/
COPY src/DrifterApps.Holefeeder.Common.IoC/*.csproj ./DrifterApps.Holefeeder.Common.IoC/
COPY src/DrifterApps.Holefeeder.ResourcesAccess.Contracts/*.csproj ./DrifterApps.Holefeeder.ResourcesAccess.Contracts/
COPY src/DrifterApps.Holefeeder.ResourcesAccess.Mongo/*.csproj ./DrifterApps.Holefeeder.ResourcesAccess.Mongo/
COPY src/DrifterApps.Holefeeder.Services.BudgetApi/*.csproj ./DrifterApps.Holefeeder.Services.BudgetApi/
COPY src/DrifterApps.Holefeeder.Services.BudgetDto/*.csproj ./DrifterApps.Holefeeder.Services.BudgetDto/
COPY src/DrifterApps.Holefeeder.ServicesHosts.BudgetApi/*.csproj ./DrifterApps.Holefeeder.ServicesHosts.BudgetApi/

RUN dotnet restore DrifterApps.Holefeeder.sln

### for caching purposes, everything above this line should be the same in all dockerfiles of the solution

COPY ./src/. ./
WORKDIR /src/DrifterApps.Holefeeder.ServicesHosts.BudgetApi/
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
WORKDIR /src/DrifterApps.Holefeeder.ServicesHosts.BudgetApi/
RUN dotnet publish --no-restore --configuration Release --output /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DrifterApps.Holefeeder.ServicesHosts.BudgetApi.dll"]
