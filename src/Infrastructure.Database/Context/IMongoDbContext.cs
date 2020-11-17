using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Infrastructure.Database.Schemas;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Context
{
    public interface IMongoDbContext : IDisposable
    {
        Task<IMongoCollection<AccountSchema>> GetAccountsAsync(CancellationToken cancellationToken = default);
        Task<IMongoCollection<CashflowSchema>> GetCashflowsAsync(CancellationToken cancellationToken = default);
        Task<IMongoCollection<CategorySchema>> GetCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IMongoCollection<TransactionSchema>> GetTransactionsAsync(CancellationToken cancellationToken = default);
        void AddCommand(Func<Task> func);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void ClearCommands();
    }
}
