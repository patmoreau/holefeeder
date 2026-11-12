using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Categories.Core;

internal interface ICategories
{
    Task<Result<IReadOnlyList<Category>>> ListAsync(User user, CancellationToken cancellationToken);

    Task<Result<bool>> IsNameUsedAsync(User user, CategoryName name, Id exceptId, CancellationToken cancellationToken);

    Task<Result<Nothing>> SaveAsync(User user, Category category, CancellationToken cancellationToken);
}
