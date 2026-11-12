using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.MyData.TestKit;

internal sealed class MyDataInMemory : IMyDataStore
{
    private readonly Dictionary<Id, MyDataSet> _data = [];

    public MyDataInMemory With(User user, MyDataSet data)
    {
        _data[user.Id] = data;
        return this;
    }

    public Task<Result<MyDataSet>> ExportAsync(User user, CancellationToken cancellationToken) =>
        Task.FromResult<Result<MyDataSet>>(_data.TryGetValue(user.Id, out var data) ? data : new MyDataSet([], [], [], []));
}
