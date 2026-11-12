using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Users.Core;

internal static class RegisteredUserErrors
{
    public const string CodeNotFound = "registered-user-not-found";

    public static ResultError NotFound => new NotFoundError(CodeNotFound, "No registered user matches the caller.");
}
