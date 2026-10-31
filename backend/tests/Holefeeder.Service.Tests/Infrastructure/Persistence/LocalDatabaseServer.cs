namespace Holefeeder.Service.Tests.Infrastructure.Persistence;

public sealed class LocalDatabaseServer : IDatabaseServer
{
    private LocalDatabaseServer(string connectionString) => ConnectionString = connectionString;

    public string ConnectionString { get; }

    public Task StartAsync() => Task.CompletedTask;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public static LocalDatabaseServer CreateServer(string connectionString) => new(connectionString);
}
