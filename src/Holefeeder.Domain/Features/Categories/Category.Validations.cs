using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.Categories;

public sealed partial record Category
{
    private static Func<Result<Nothing>> IdValidation(CategoryId id) =>
        () => id != CategoryId.Empty
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(CategoryErrors.IdRequired);

    private static Func<Result<Nothing>> NameValidation(string name) =>
        () => !string.IsNullOrWhiteSpace(name) && name.Length <= 255
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(CategoryErrors.NameRequired);

    private static Func<Result<Nothing>> UserIdValidation(Guid id) =>
        () => id != Guid.Empty
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(CategoryErrors.UserIdRequired);
}
