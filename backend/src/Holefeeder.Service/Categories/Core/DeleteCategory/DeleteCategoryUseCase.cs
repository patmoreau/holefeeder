using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Categories.Core.DeleteCategory;

internal sealed class DeleteCategoryUseCase(ICategories categories)
{
    public async Task<Result<Nothing>> HandleAsync(DeleteCategoryRequest request, CancellationToken cancellationToken)
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

        return await categories.SaveAsync(request.User, existing with { Inactive = true }, cancellationToken);
    }
}
