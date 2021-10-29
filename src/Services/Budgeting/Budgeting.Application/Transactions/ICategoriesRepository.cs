using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions
{
    public interface ICategoriesRepository
    {
        Task<CategoryViewModel> FindByNameAsync(Guid userId, string name, CancellationToken cancellationToken);
    }
}
