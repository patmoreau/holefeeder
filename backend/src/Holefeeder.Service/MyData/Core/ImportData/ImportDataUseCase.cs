using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.MyData.Core.ImportData;

internal sealed class ImportDataUseCase(IMyDataStore store, IImportStatuses statuses)
{
    public async Task<Result<Nothing>> HandleAsync(Id importId, ImportDataRequest request, CancellationToken cancellationToken)
    {
        var run = new ImportRun(statuses, importId, request.User.Id);

        var result = await ImportAsync(request, run, cancellationToken);
        run.Finish(result);

        return result;
    }

    private async Task<Result<Nothing>> ImportAsync(ImportDataRequest request, ImportRun run, CancellationToken cancellationToken)
    {
        var importResult = await store.BeginImportAsync(request.User, cancellationToken);
        if (importResult.IsFailure)
        {
            return importResult.Error;
        }

        await using var import = importResult.Value;
        var data = request.Data;
        var tables = new Func<Task<Result<Nothing>>>[]
        {
            () => ImportTableAsync(MyDataTable.Accounts, data.Accounts, account => account.Id, import.SaveAsync),
            () => ImportTableAsync(MyDataTable.Categories, data.Categories, category => category.Id, import.SaveAsync),
            () => ImportTableAsync(MyDataTable.Cashflows, data.Cashflows, cashflow => cashflow.Id, import.SaveAsync),
            () => ImportTableAsync(MyDataTable.Transactions, data.Transactions, transaction => transaction.Id, import.SaveAsync)
        };

        foreach (var importTable in tables)
        {
            var imported = await importTable();
            if (imported.IsFailure)
            {
                return imported.Error;
            }
        }

        return await import.CommitAsync(cancellationToken);

        async Task<Result<Nothing>> ImportTableAsync<T>(
            MyDataTable table, IReadOnlyList<T> records, Func<T, Id> idOf, Func<T, CancellationToken, Task<Result<Nothing>>> save)
        {
            if (records.Count == 0)
            {
                return Nothing.Value;
            }

            run.Start(table, records.Count);
            foreach (var record in records)
            {
                var exists = await import.ExistsAsync(table, idOf(record), cancellationToken);
                if (exists.IsFailure)
                {
                    return exists.Error;
                }

                if (!exists.Value || request.UpdateExisting)
                {
                    var saved = await save(record, cancellationToken);
                    if (saved.IsFailure)
                    {
                        return saved.Error;
                    }
                }

                run.Advance(table);
            }

            return Nothing.Value;
        }
    }

    private sealed class ImportRun
    {
        private readonly IImportStatuses _statuses;
        private ImportStatus _status;

        public ImportRun(IImportStatuses statuses, Id importId, Id userId)
        {
            _statuses = statuses;
            _status = ImportStatus.Initializing(importId, userId) with { State = ImportState.InProgress };
            _statuses.Save(_status);
        }

        public void Start(MyDataTable table, int total) => Save(_status.WithProgress(table, new ImportProgress(0, total)));

        public void Advance(MyDataTable table) => Save(_status.WithProgress(table, _status.ProgressOf(table).Advanced()));

        public void Finish(Result<Nothing> result) =>
            Save(result.IsSuccess
                ? _status with { State = ImportState.Completed }
                : _status with { State = ImportState.Error, Message = $"{result.Error.Code}: {result.Error.Description}" });

        private void Save(ImportStatus status)
        {
            _status = status;
            _statuses.Save(status);
        }
    }
}
