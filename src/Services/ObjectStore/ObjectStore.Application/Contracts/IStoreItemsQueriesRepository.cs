using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Contracts
{
    public interface IStoreItemsQueriesRepository
    {
        Task<QueryResult<StoreItemViewModel>> FindAsync(Guid userId, QueryParams queryParams,
            CancellationToken cancellationToken);

        Task<StoreItemViewModel> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);

        Task<bool> AnyCodeAsync(Guid userId, string code, CancellationToken cancellationToken);

        Task<bool> AnyIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
    }
}
