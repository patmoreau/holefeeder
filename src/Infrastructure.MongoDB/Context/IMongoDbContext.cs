using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Schemas;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB.Context
{
    public interface IMongoDbContext : IDisposable
    {
        Task<IMongoCollection<AccountSchema>> GetAccountsAsync(CancellationToken cancellationToken);
        Task<IMongoCollection<CashflowSchema>> GetCashflowsAsync(CancellationToken cancellationToken);
        Task<IMongoCollection<CategorySchema>> GetCategoriesAsync(CancellationToken cancellationToken);
        Task<IMongoCollection<TransactionSchema>> GetTransactionsAsync(CancellationToken cancellationToken);
        Task<IMongoCollection<UserSchema>> GetUsersAsync(CancellationToken cancellationToken);
        void AddCommand(Func<Task> func);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        void ClearCommands();
    }
}
