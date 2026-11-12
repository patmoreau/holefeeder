using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.MyData.Core;

internal enum ImportState
{
    Initializing,
    InProgress,
    Completed,
    Error
}

internal sealed record ImportProgress(int Processed, int Total)
{
    public static ImportProgress None => new(0, 0);

    public ImportProgress Advanced() => this with { Processed = Processed + 1 };
}

internal sealed record ImportStatus(
    Id ImportId,
    Id UserId,
    ImportState State,
    ImportProgress Accounts,
    ImportProgress Categories,
    ImportProgress Cashflows,
    ImportProgress Transactions,
    string Message)
{
    public static ImportStatus Initializing(Id importId, Id userId) =>
        new(importId, userId, ImportState.Initializing, ImportProgress.None, ImportProgress.None, ImportProgress.None, ImportProgress.None,
            string.Empty);

    public ImportProgress ProgressOf(MyDataTable table) =>
        table switch
        {
            MyDataTable.Accounts => Accounts,
            MyDataTable.Categories => Categories,
            MyDataTable.Cashflows => Cashflows,
            _ => Transactions
        };

    public ImportStatus WithProgress(MyDataTable table, ImportProgress progress) =>
        table switch
        {
            MyDataTable.Accounts => this with { Accounts = progress },
            MyDataTable.Categories => this with { Categories = progress },
            MyDataTable.Cashflows => this with { Cashflows = progress },
            _ => this with { Transactions = progress }
        };
}

internal interface IImportStatuses
{
    void Save(ImportStatus status);

    Result<ImportStatus> Find(Id importId, Id userId);
}

internal static class ImportStatusErrors
{
    public const string CodeNotFound = "my-data-import-not-found";

    public static ResultError NotFound => new NotFoundError(CodeNotFound, "Import was not found.");
}
