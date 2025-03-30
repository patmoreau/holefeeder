using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Domain.Features.Accounts;

public sealed partial record Account
{
    private static Func<Result<Nothing>> IdValidation(AccountId value) =>
        () => value != AccountId.Empty
            ? Nothing.Value
            : AccountErrors.IdRequired;

    private static Func<Result<Nothing>> NameValidation(string value) =>
        () => !string.IsNullOrWhiteSpace(value) && value.Length <= 100
            ? Nothing.Value
            : AccountErrors.NameRequired;

    private static Func<Result<Nothing>> OpenDateValidation(DateOnly value) =>
        () => value != default
            ? Nothing.Value
            : AccountErrors.OpenDateRequired;

    private static Func<Result<Nothing>> UserIdValidation(UserId value) =>
        () => value != UserId.Empty
            ? Nothing.Value
            : AccountErrors.UserIdRequired;
}
