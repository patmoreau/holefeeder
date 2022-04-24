using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Domain.Features.Categories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<Category?> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken);
    Task SaveAsync(Category category, CancellationToken cancellationToken);
}
