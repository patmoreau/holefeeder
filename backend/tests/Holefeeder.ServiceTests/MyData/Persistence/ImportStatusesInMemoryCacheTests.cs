using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.MyData.Persistence;

using Microsoft.Extensions.Caching.Memory;

namespace Holefeeder.ServiceTests.MyData.Persistence;

public sealed class ImportStatusesInMemoryCacheTests : IDisposable
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    [Fact]
    public void Find_ReturnsTheLastSavedStatus()
    {
        var statuses = new ImportStatusesInMemoryCache(_cache);
        var status = ImportStatus.Initializing(AnId(), AnId());
        var completed = status with { State = ImportState.Completed };

        statuses.Save(status);
        statuses.Save(completed);

        statuses.Find(status.ImportId, status.UserId).Should().BeSuccessful().And.WithValue(completed);
    }

    [Fact]
    public void Find_FailsWithNotFound_ForAnotherUser()
    {
        var statuses = new ImportStatusesInMemoryCache(_cache);
        var status = ImportStatus.Initializing(AnId(), AnId());
        statuses.Save(status);

        statuses.Find(status.ImportId, AnId()).Should().BeFailure().And.WithError(ImportStatusErrors.NotFound);
    }

    [Fact]
    public void Find_FailsWithNotFound_WhenUnknown() =>
        new ImportStatusesInMemoryCache(_cache).Find(AnId(), AnId()).Should().BeFailure().And.WithError(ImportStatusErrors.NotFound);

    public void Dispose() => _cache.Dispose();
}
