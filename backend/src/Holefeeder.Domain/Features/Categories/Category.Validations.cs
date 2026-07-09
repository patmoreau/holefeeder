using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.Categories;

public sealed partial record Category
{
    private static Func<Result<Nothing>> IdValidation(CategoryId id) =>
        () => id != CategoryId.Empty
            ? Nothing.Value
            : CategoryErrors.IdRequired;

    private static Func<Result<Nothing>> NameValidation(string name) =>
        () => !string.IsNullOrWhiteSpace(name) && name.Length <= 255
            ? Nothing.Value
            : CategoryErrors.NameRequired;

    private static Func<Result<Nothing>> UserIdValidation(Guid id) =>
        () => id != Guid.Empty
            ? Nothing.Value
            : CategoryErrors.UserIdRequired;
}
