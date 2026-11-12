using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Categories.Core.ListCategories;

internal sealed class ListCategoriesUseCase(ICategories categories)
{
    public Task<Result<IReadOnlyList<Category>>> HandleAsync(ListCategoriesRequest request, CancellationToken cancellationToken) =>
        categories.ListAsync(request.User, cancellationToken);
}
