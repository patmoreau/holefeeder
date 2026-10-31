namespace Holefeeder.Service.Tests.Infrastructure.Persistence;

public interface IDatabaseServer : IAsyncDisposable
{
    string ConnectionString { get; }

    Task StartAsync();
}
