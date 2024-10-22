using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.Categories;

public static class CategoryErrors
{
    public const string CodeIdRequired = $"{nameof(Category)}.{nameof(Category.Id)}";
    public const string CodeNameRequired = $"{nameof(Category)}.{nameof(Category.Name)}";
    public const string CodeUserIdRequired = $"{nameof(Category)}.{nameof(Category.UserId)}";

    public static ResultError IdRequired => new(CodeIdRequired, "Id is required");
    public static ResultError NameRequired => new(CodeNameRequired, "Name must be from 1 to 255 characters");
    public static ResultError UserIdRequired => new(CodeUserIdRequired, "UserId is required");
}
