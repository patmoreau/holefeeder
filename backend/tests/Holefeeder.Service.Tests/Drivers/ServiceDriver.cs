using Holefeeder.Service.Tests.Infrastructure.Persistence;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Holefeeder.Service.Tests.Drivers;

public sealed class ServiceDriver : WebApplicationFactory<Api>, IAsyncLifetime
{
    private const string DatabaseName = "holefeeder_service_tests";
    private const string ConnectionStringKey = "ConnectionStrings:BudgetingConnectionString";

    private readonly IDatabaseServer _databaseServer;

    public ServiceDriver()
    {
        var connectionString = Environment.GetEnvironmentVariable("LOCAL_CONNECTION_STRING");
        _databaseServer = string.IsNullOrWhiteSpace(connectionString)
            ? PostgreDatabaseServer.CreateServer(DatabaseName)
            : LocalDatabaseServer.CreateServer(connectionString);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("SERVICE_TESTS");
        builder.UseSetting(ConnectionStringKey, _databaseServer.ConnectionString);
    }

    public async ValueTask InitializeAsync() => await _databaseServer.StartAsync();

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _databaseServer.DisposeAsync();
    }
}
