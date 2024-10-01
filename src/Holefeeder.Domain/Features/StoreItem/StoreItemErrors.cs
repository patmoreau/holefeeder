namespace Holefeeder.Domain.Features.StoreItem;

public static class StoreItemErrors
{
    public const string CodeCodeAlreadyExists = $"{nameof(StoreItem)}.{nameof(CodeAlreadyExists)}";
    public const string CodeNotFound = $"{nameof(StoreItem)}.{nameof(NotFound)}";

    public static ResultError IdRequired => new("StoreItem.IdRequired", "Id is required");
    public static ResultError CodeRequired => new("StoreItem.CodeRequired", "Code is required");

    public static ResultError UserIdRequired => new("StoreItem.UserIdRequired", "UserId is required");

    public static ResultError NotFound(StoreItemId id) => new(CodeNotFound, $"StoreItem '{id}' not found");

    public static ResultError CodeAlreadyExists(string code) =>
        new(CodeCodeAlreadyExists, $"Code '{code}' already exists.");
}
