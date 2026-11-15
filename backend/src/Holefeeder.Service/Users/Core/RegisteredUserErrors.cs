using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Users.Core;

internal static class RegisteredUserErrors
{
    public const string CodeNotFound = "registered-user-not-found";
    public const string CodeAlreadyRegistered = "user-already-registered";

    public static ResultError NotFound => new NotFoundError(CodeNotFound);
    public static ResultError AlreadyRegistered => new(CodeAlreadyRegistered);
}
