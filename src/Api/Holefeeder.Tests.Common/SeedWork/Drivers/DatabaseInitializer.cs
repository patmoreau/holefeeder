using System.Data;
using System.Data.Common;
using Respawn;
using Xunit;

namespace Holefeeder.Tests.Common.SeedWork.Drivers;

public abstract class DatabaseInitializer : IAsyncLifetime, IAsyncDisposable
{
    private Respawner _respawner = default!;

    protected abstract DbConnection Connection { get; }
    protected abstract RespawnerOptions Options { get; }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await Connection.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public virtual async Task InitializeAsync()
    {
        await Connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(Connection, Options);
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;

    public async Task ResetCheckpoint()
    {
        if (Connection.State is ConnectionState.Closed or ConnectionState.Broken)
        {
            await InitializeAsync();
        }

        await _respawner.ResetAsync(Connection);
    }
}
