using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Accounts.Core;

internal sealed record AccountName
{
    private AccountName(string value) => Value = value;

    public string Value { get; }

    public static Result<AccountName> Create(string? value) =>
        string.IsNullOrWhiteSpace(value) || value.Length > 100
            ? AccountNameErrors.Invalid
            : new AccountName(value);

    public static AccountName Valid(string value) => new(value);
}

internal static class AccountNameErrors
{
    public const string CodeInvalid = "account-name-invalid";
    public const string CodeAlreadyExists = "account-name-already-exists";

    public static ResultError Invalid => new(CodeInvalid, "Account name must be between 1 and 100 characters.");

    public static ResultError AlreadyExists => new(CodeAlreadyExists, "Account name already exists.");
}
