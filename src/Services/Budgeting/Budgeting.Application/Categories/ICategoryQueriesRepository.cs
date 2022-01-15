using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

namespace DrifterApps.Holefeeder.Budgeting.Application.Categories
{
    public interface ICategoryQueriesRepository
    {
        Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(Guid userId,
            CancellationToken cancellationToken = default);
    }
}
