using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.Accounts;

public static class AccountErrors
{
    public const string CodeIdRequired = $"{nameof(Account)}.{nameof(Account.Id)}";
    public const string CodeNameRequired = $"{nameof(Account)}.{nameof(Account.Name)}";
    public const string CodeOpenDateRequired = $"{nameof(Account)}.{nameof(Account.OpenDate)}";
    public const string CodeUserIdRequired = $"{nameof(Account)}.{nameof(Account.UserId)}";
    public const string CodeAccountClosed = $"{nameof(Account)}.{nameof(AccountClosed)}";
    public const string CodeActiveCashflows = $"{nameof(Account)}.{nameof(ActiveCashflows)}";
    public const string CodeNameAlreadyExists = $"{nameof(Account)}.{nameof(NameAlreadyExists)}";
    public const string CodeNotFound = $"{nameof(Account)}.{nameof(NotFound)}";

    public static ResultError IdRequired => new(CodeIdRequired, "Id is required");
    public static ResultError NameRequired => new(CodeNameRequired, "Name must be from 1 to 100 characters");
    public static ResultError OpenDateRequired => new(CodeOpenDateRequired, "OpenDate is required");
    public static ResultError UserIdRequired => new(CodeUserIdRequired, "UserId is required");
    public static ResultError AccountClosed => new(CodeAccountClosed, "Account already closed");
    public static ResultError ActiveCashflows => new(CodeActiveCashflows, "Account has active cashflows");
    public static ResultError NameAlreadyExists(string name) => new(CodeNameAlreadyExists, $"Name '{name}' already exists.");
    public static ResultError NotFound(AccountId id) => new(CodeNotFound, $"Account '{id}' not found");
}
