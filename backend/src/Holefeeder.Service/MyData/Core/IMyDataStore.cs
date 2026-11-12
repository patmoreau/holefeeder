using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.MyData.Core;

internal interface IMyDataStore
{
    Task<Result<MyDataSet>> ExportAsync(User user, CancellationToken cancellationToken);

    Task<Result<IMyDataImport>> BeginImportAsync(User user, CancellationToken cancellationToken);
}

internal interface IMyDataImport : IAsyncDisposable
{
    Task<Result<bool>> ExistsAsync(MyDataTable table, Id id, CancellationToken cancellationToken);

    Task<Result<Nothing>> SaveAsync(MyDataAccount account, CancellationToken cancellationToken);

    Task<Result<Nothing>> SaveAsync(MyDataCategory category, CancellationToken cancellationToken);

    Task<Result<Nothing>> SaveAsync(MyDataCashflow cashflow, CancellationToken cancellationToken);

    Task<Result<Nothing>> SaveAsync(MyDataTransaction transaction, CancellationToken cancellationToken);

    Task<Result<Nothing>> CommitAsync(CancellationToken cancellationToken);
}

internal static class MyDataImportErrors
{
    public const string CodeIdTaken = "my-data-id-taken";

    public static ResultError IdTaken(MyDataTable table) =>
        new(CodeIdTaken, $"An item of {table.ToString().ToLowerInvariant()} with this id belongs to another user.");
}
