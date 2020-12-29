using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Contracts
{
    public interface IStoreQueriesRepository
    {
        Task<IEnumerable<StoreItemViewModel>> GetItemsAsync(Guid userId, QueryParams queryParams, CancellationToken cancellationToken = default);
        
        Task<StoreItemViewModel> GetItemAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);

        Task<bool> AnyCodeAsync(Guid userId, string code, CancellationToken cancellationToken = default);

        Task<bool> AnyIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
    }
}
