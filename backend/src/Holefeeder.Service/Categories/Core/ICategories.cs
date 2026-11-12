using DrifterApps.Seeds.FluentResult;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Categories.Core;

internal interface ICategories
{
    Task<Result<IReadOnlyList<Category>>> ListAsync(User user, CancellationToken cancellationToken);
}
