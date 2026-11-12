using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;
using UserErrors = Holefeeder.Service.Support.Core.UserErrors;

namespace Holefeeder.Service.Categories.Core.CreateCategory;

internal sealed class CreateCategoryUseCase(ICategories categories)
{
    public async Task<Result<Id>> HandleAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        if (request.User == User.NoUser)
        {
            return UserErrors.NotRegistered;
        }

        var category = new Category(Id.New(), request.Name, request.Type, request.Color, request.Budget, request.Favorite, System: false);

        var nameUsedResult = await categories.IsNameUsedAsync(request.User, category.Name, category.Id, cancellationToken);
        if (nameUsedResult.IsFailure)
        {
            return nameUsedResult.Error;
        }
        if (nameUsedResult.Value)
        {
            return CategoryErrors.NameTaken;
        }

        var saveResult = await categories.SaveAsync(request.User, category, cancellationToken);
        if (saveResult.IsFailure)
        {
            return saveResult.Error;
        }

        return category.Id;
    }
}
