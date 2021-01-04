using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Schemas;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context
{
    public interface IMongoDbContext : IDisposable
    {
        Task<IMongoCollection<StoreItemSchema>> GetStoreItemsAsync(CancellationToken cancellationToken = default);
        void AddCommand(Func<Task> func);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void ClearCommands();
    }
}
