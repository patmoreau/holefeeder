using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.StoreItem;

public static class StoreItemErrors
{
    public const string CodeIdRequired = $"{nameof(StoreItem)}.{nameof(StoreItem.Id)}";
    public const string CodeCodeRequired = $"{nameof(StoreItem)}.{nameof(StoreItem.Code)}";
    public const string CodeUserIdRequired = $"{nameof(StoreItem)}.{nameof(StoreItem.UserId)}";
    public const string CodeCodeAlreadyExists = $"{nameof(StoreItem)}.{nameof(CodeAlreadyExists)}";
    public const string CodeNotFound = $"{nameof(StoreItem)}.{nameof(NotFound)}";

    public static ResultError IdRequired => new(CodeIdRequired, "Id is required");
    public static ResultError CodeRequired => new(CodeCodeRequired, "Code is required");

    public static ResultError UserIdRequired => new(CodeUserIdRequired, "UserId is required");

    public static ResultError NotFound(StoreItemId id) => new(CodeNotFound, $"StoreItem '{id}' not found");

    public static ResultError CodeAlreadyExists(string code) =>
        new(CodeCodeAlreadyExists, $"Code '{code}' already exists.");
}
