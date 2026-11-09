using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.StoreItems.Core;

internal sealed record StoreItem(Id Id, Code Code, string Data)
{
    public static Result<StoreItem> Create(Id id, Code code, string data) => new StoreItem(id, code, data);

    public static StoreItem Valid(Id id, Code code, string data) => new(id, code, data);
}

internal static class StoreItemErrors
{
    public const string CodeNotFound = "store-item-not-found";
    public const string CodeCodeAlreadyExists = "store-item-code-already-exists";
    public const string CodeUserNotRegistered = "user-not-registered";

    public static NotFoundError NotFound => new(CodeNotFound, "Store item was not found.");

    public static ResultError CodeAlreadyExists => new(CodeCodeAlreadyExists, "Store item code already exists.");

    public static ResultError UserNotRegistered => new(CodeUserNotRegistered, "User is not registered.");
}
