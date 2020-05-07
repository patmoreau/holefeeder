FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
LABEL author="Drifter Apps Inc."
WORKDIR /app
EXPOSE 5001
COPY publish/DrifterApps.Holefeeder.ServicesHosts.BudgetApi ./
ENTRYPOINT ["dotnet", "DrifterApps.Holefeeder.ServicesHosts.BudgetApi.dll"]
