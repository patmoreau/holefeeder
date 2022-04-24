using Holefeeder.Application.Models;

namespace Holefeeder.Application.Features.Transactions;

public interface ICategoriesRepository
{
    Task<CategoryViewModel?> FindByNameAsync(Guid userId, string name, CancellationToken cancellationToken);
}
