using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.CategoryContext;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<Category?> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken);
    Task SaveAsync(Category category, CancellationToken cancellationToken);
}
