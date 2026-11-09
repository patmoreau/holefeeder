using Dapper;

using DotNet.Testcontainers.Builders;

using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.SeedWork;

using Microsoft.Extensions.Logging.Abstractions;

using Npgsql;

using Testcontainers.PostgreSql;

namespace Holefeeder.ServiceTests.TestKit.Persistence;

public sealed class DatabaseForTest : IAsyncLifetime
{
    private readonly string? _localConnectionString = Environment.GetEnvironmentVariable("LOCAL_CONNECTION_STRING");

    private readonly PostgreSqlContainer? _container;

    public DatabaseForTest()
    {
        if (string.IsNullOrWhiteSpace(_localConnectionString))
        {
            _container = new PostgreSqlBuilder("postgres:18")
                .WithWaitStrategy(Wait.ForUnixContainer())
                .Build();
        }
    }

    public string ConnectionString => _container?.GetConnectionString() ?? _localConnectionString!;

    public NpgsqlDataSource DataSource => field ??= NpgsqlDataSource.Create(ConnectionString);

    public NpgsqlDataSource BadDataSource { get; } = NpgsqlDataSource.Create("Host=127.0.0.1;Port=1;Username=test;Timeout=1");

    public async ValueTask InitializeAsync()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        if (_container is not null)
        {
            await _container.StartAsync().ConfigureAwait(false);
        }

        WebApplicationExtensions.MigrateDb(
            new BudgetingConnectionStringBuilder { ConnectionString = ConnectionString },
            NullLogger.Instance);
    }

    public async ValueTask DisposeAsync()
    {
        await DataSource.DisposeAsync().ConfigureAwait(false);
        await BadDataSource.DisposeAsync().ConfigureAwait(false);
        if (_container is not null)
        {
            await _container.DisposeAsync().ConfigureAwait(false);
        }
    }
}
