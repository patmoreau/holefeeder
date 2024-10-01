namespace Holefeeder.Domain.Features.Categories;

public static class CategoryErrors
{
    public const string CodeIdRequired = $"{nameof(Category)}.{nameof(IdRequired)}";
    public const string CodeNameRequired = $"{nameof(Category)}.{nameof(NameRequired)}";
    public const string CodeUserIdRequired = $"{nameof(Category)}.{nameof(UserIdRequired)}";

    public static ResultError IdRequired => new(CodeIdRequired, "Id is required");
    public static ResultError NameRequired => new(CodeNameRequired, "Name must be from 1 to 255 characters");
    public static ResultError UserIdRequired => new(CodeUserIdRequired, "UserId is required");
}
