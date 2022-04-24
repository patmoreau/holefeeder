using Holefeeder.Application.Models;

namespace Holefeeder.Application.Features.Categories;

public interface ICategoryQueriesRepository
{
    Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(Guid userId, CancellationToken cancellationToken = default);
}
