using DotNet.Testcontainers.Builders;

using Testcontainers.PostgreSql;

namespace Holefeeder.Service.Tests.Infrastructure.Persistence;

public sealed class PostgreDatabaseServer : IDatabaseServer
{
    private const string RootUser = "root";
    private const string RootPassword = "root_password";
    private readonly PostgreSqlContainer _container;

    private PostgreDatabaseServer(string databaseName, string image) =>
        _container = new PostgreSqlBuilder(image)
            .WithDatabase(databaseName)
            .WithUsername(RootUser)
            .WithPassword(RootPassword)
            .WithWaitStrategy(Wait.ForUnixContainer())
            .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task StartAsync() => await _container.StartAsync().ConfigureAwait(false);

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public static PostgreDatabaseServer CreateServer(string databaseName, string image = "postgres:18") =>
        new(databaseName, image);
}
