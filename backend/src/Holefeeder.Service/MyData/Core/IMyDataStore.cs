using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.MyData.Core;

internal interface IMyDataStore
{
    Task<Result<MyDataSet>> ExportAsync(User user, CancellationToken cancellationToken);
}
