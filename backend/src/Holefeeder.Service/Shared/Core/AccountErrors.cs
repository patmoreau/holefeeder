namespace Holefeeder.Service.Shared.Core;

internal static class AccountErrors
{
    public const string CodeNotFound = "account-not-found";

    public static NotFoundError NotFound => new(CodeNotFound, "Account was not found.");
}
