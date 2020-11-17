using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Transactions.Models;

namespace DrifterApps.Holefeeder.Application.Transactions.Contracts
{
    public interface ICategoryQueries
    {
        Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
