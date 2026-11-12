using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Categories.Core.ModifyCategory;

internal sealed class ModifyCategoryUseCase(ICategories categories)
{
    public async Task<Result<Nothing>> HandleAsync(ModifyCategoryRequest request, CancellationToken cancellationToken)
    {
        var existingResult = await categories.GetAsync(request.User, request.Id, cancellationToken);
        if (existingResult.IsFailure)
        {
            return existingResult.Error;
        }
        var existing = existingResult.Value;

        if (existing.System)
        {
            return CategoryErrors.System;
        }

        var nameUsedResult = await categories.IsNameUsedAsync(request.User, request.Name, existing.Id, cancellationToken);
        if (nameUsedResult.IsFailure)
        {
            return nameUsedResult.Error;
        }
        if (nameUsedResult.Value)
        {
            return CategoryErrors.NameTaken;
        }

        var modified = existing with
        {
            Name = request.Name,
            Type = request.Type,
            Color = request.Color,
            Budget = request.Budget,
            Favorite = request.Favorite
        };

        return await categories.SaveAsync(request.User, modified, cancellationToken);
    }
}
