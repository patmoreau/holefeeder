using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

namespace DrifterApps.Holefeeder.ObjectStore.Application.StoreItems
{
    public interface IStoreItemsQueriesRepository
    {
        Task<(int Total, IEnumerable<StoreItemViewModel> Items)> FindAsync(Guid userId, QueryParams queryParams,
            CancellationToken cancellationToken);

        Task<StoreItemViewModel?> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);

        Task<bool> AnyCodeAsync(Guid userId, string code, CancellationToken cancellationToken);

        Task<bool> AnyIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
    }
}
