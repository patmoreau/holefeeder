namespace Holefeeder.Domain.Features.Categories;

public sealed partial record Category
{
    private static ResultValidation IdValidation(CategoryId id) =>
        ResultValidation.Create(() => id != CategoryId.Empty, CategoryErrors.IdRequired);

    private static ResultValidation NameValidation(string name) =>
        ResultValidation.Create(() => !string.IsNullOrWhiteSpace(name) && name.Length <= 255, CategoryErrors.NameRequired);

    private static ResultValidation UserIdValidation(Guid id) =>
        ResultValidation.Create(() => id != Guid.Empty, CategoryErrors.UserIdRequired);
}
