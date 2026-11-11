using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;

internal sealed record AccountType
{
    public static readonly AccountType Checking = new(nameof(Checking), 1);
    public static readonly AccountType Investment = new(nameof(Investment), 1);
    public static readonly AccountType Savings = new(nameof(Savings), 1);
    public static readonly AccountType CreditCard = new(nameof(CreditCard), -1);
    public static readonly AccountType CreditLine = new(nameof(CreditLine), -1);
    public static readonly AccountType Loan = new(nameof(Loan), -1);
    public static readonly AccountType Mortgage = new(nameof(Mortgage), -1);

    private static readonly AccountType[] All = [Checking, Investment, Savings, CreditCard, CreditLine, Loan, Mortgage];

    private AccountType(string name, int sign) => (Name, Sign) = (name, sign);

    public string Name { get; }

    public int Sign { get; }

    public bool IsAsset => Sign > 0;

    public static Result<AccountType> Create(string? value) =>
        All.FirstOrDefault(type => string.Equals(type.Name, value, StringComparison.OrdinalIgnoreCase)) is { } accountType
            ? accountType
            : AccountTypeErrors.Invalid;

    public static AccountType Valid(string value) =>
        All.First(type => string.Equals(type.Name, value, StringComparison.OrdinalIgnoreCase));
}

internal static class AccountTypeErrors
{
    public const string CodeInvalid = "account-type-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Account type is invalid.");
}
