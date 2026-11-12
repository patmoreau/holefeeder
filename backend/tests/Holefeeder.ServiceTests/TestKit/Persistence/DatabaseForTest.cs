using Dapper;

using DotNet.Testcontainers.Builders;

using Holefeeder.Service.Support.Persistence.Schema;

using Microsoft.Extensions.Logging.Abstractions;

using Npgsql;

using Testcontainers.PostgreSql;

namespace Holefeeder.ServiceTests.TestKit.Persistence;

public sealed class DatabaseForTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18")
        .WithWaitStrategy(Wait.ForUnixContainer())
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public NpgsqlDataSource DataSource => field ??= NpgsqlDataSource.Create(ConnectionString);

    public NpgsqlDataSource BadDataSource { get; } = NpgsqlDataSource.Create("Host=127.0.0.1;Port=1;Username=test;Timeout=1");

    public async ValueTask InitializeAsync()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        await _container.StartAsync().ConfigureAwait(false);

        SchemaMigrations.MigrateDb(ConnectionString, NullLogger.Instance);
    }

    public async ValueTask DisposeAsync()
    {
        await DataSource.DisposeAsync().ConfigureAwait(false);
        await BadDataSource.DisposeAsync().ConfigureAwait(false);
        await _container.DisposeAsync().ConfigureAwait(false);
    }
}
