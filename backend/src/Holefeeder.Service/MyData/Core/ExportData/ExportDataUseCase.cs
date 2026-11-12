using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.MyData.Core.ExportData;

internal sealed class ExportDataUseCase(IMyDataStore store)
{
    public Task<Result<MyDataSet>> HandleAsync(ExportDataRequest request, CancellationToken cancellationToken) =>
        store.ExportAsync(request.User, cancellationToken);
}
