using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Domain.Features.Accounts;

public sealed partial record Account
{
    private static ResultValidation IdValidation(AccountId value) =>
        ResultValidation.Create(() => value != AccountId.Empty, AccountErrors.IdRequired);

    private static ResultValidation NameValidation(string value) =>
        ResultValidation.Create(() => !string.IsNullOrWhiteSpace(value) && value.Length <= 100, AccountErrors.NameRequired);

    private static ResultValidation OpenDateValidation(DateOnly value) =>
        ResultValidation.Create(() => value != default, AccountErrors.OpenDateRequired);

    private static ResultValidation UserIdValidation(UserId value) =>
        ResultValidation.Create(() => value != UserId.Empty, AccountErrors.UserIdRequired);
}
