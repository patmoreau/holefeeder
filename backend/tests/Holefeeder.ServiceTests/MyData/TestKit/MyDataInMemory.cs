using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.MyData.TestKit;

internal sealed class MyDataInMemory : IMyDataStore
{
    private readonly Dictionary<(MyDataTable Table, Id Id), (Id Owner, object Record)> _rows = [];

    public MyDataInMemory With(User user, MyDataSet data)
    {
        foreach (var account in data.Accounts)
        {
            _rows[(MyDataTable.Accounts, account.Id)] = (user.Id, account);
        }
        foreach (var category in data.Categories)
        {
            _rows[(MyDataTable.Categories, category.Id)] = (user.Id, category);
        }
        foreach (var cashflow in data.Cashflows)
        {
            _rows[(MyDataTable.Cashflows, cashflow.Id)] = (user.Id, cashflow);
        }
        foreach (var transaction in data.Transactions)
        {
            _rows[(MyDataTable.Transactions, transaction.Id)] = (user.Id, transaction);
        }
        return this;
    }

    public Task<Result<MyDataSet>> ExportAsync(User user, CancellationToken cancellationToken) =>
        Task.FromResult<Result<MyDataSet>>(DataOf(user));

    public Task<Result<IMyDataImport>> BeginImportAsync(User user, CancellationToken cancellationToken) =>
        Task.FromResult<Result<IMyDataImport>>(new Import(this, user));

    public MyDataSet DataOf(User user)
    {
        var records = _rows.Values.Where(row => row.Owner == user.Id).Select(row => row.Record).ToList();
        return new MyDataSet(
            [.. records.OfType<MyDataAccount>()],
            [.. records.OfType<MyDataCategory>()],
            [.. records.OfType<MyDataCashflow>()],
            [.. records.OfType<MyDataTransaction>()]);
    }

    private sealed class Import(MyDataInMemory store, User user) : IMyDataImport
    {
        private readonly Dictionary<(MyDataTable Table, Id Id), (Id Owner, object Record)> _staged = [];

        public Task<Result<bool>> ExistsAsync(MyDataTable table, Id id, CancellationToken cancellationToken) =>
            Task.FromResult<Result<bool>>(Find(table, id) is { } row && row.Owner == user.Id);

        public Task<Result<Nothing>> SaveAsync(MyDataAccount account, CancellationToken cancellationToken) =>
            Stage(MyDataTable.Accounts, account.Id, account);

        public Task<Result<Nothing>> SaveAsync(MyDataCategory category, CancellationToken cancellationToken) =>
            Stage(MyDataTable.Categories, category.Id, category);

        public Task<Result<Nothing>> SaveAsync(MyDataCashflow cashflow, CancellationToken cancellationToken) =>
            Stage(MyDataTable.Cashflows, cashflow.Id, cashflow);

        public Task<Result<Nothing>> SaveAsync(MyDataTransaction transaction, CancellationToken cancellationToken) =>
            Stage(MyDataTable.Transactions, transaction.Id, transaction);

        public Task<Result<Nothing>> CommitAsync(CancellationToken cancellationToken)
        {
            foreach (var (key, row) in _staged)
            {
                store._rows[key] = row;
            }
            return Task.FromResult<Result<Nothing>>(Nothing.Value);
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        private (Id Owner, object Record)? Find(MyDataTable table, Id id) =>
            _staged.TryGetValue((table, id), out var staged) ? staged :
            store._rows.TryGetValue((table, id), out var row) ? row : null;

        private Task<Result<Nothing>> Stage(MyDataTable table, Id id, object record)
        {
            if (Find(table, id) is { } row && row.Owner != user.Id)
            {
                return Task.FromResult<Result<Nothing>>(MyDataImportErrors.IdTaken(table));
            }

            _staged[(table, id)] = (user.Id, record);
            return Task.FromResult<Result<Nothing>>(Nothing.Value);
        }
    }
}
