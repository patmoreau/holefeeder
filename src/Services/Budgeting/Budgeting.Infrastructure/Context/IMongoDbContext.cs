using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas;

using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Context
{
    public interface IMongoDbContext : IDisposable
    {
        IMongoCollection<AccountSchema> Accounts { get; }
        IMongoCollection<CashflowSchema> Cashflows { get; }
        IMongoCollection<CategorySchema> Categories { get; }
        IMongoCollection<TransactionSchema> Transactions { get; }

        void AddCommand(Func<Task> func);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        void ClearCommands();

        void Migrate();
    }
}
