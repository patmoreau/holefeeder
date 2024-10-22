using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Domain.Features.Transactions;

public static class TransactionErrors
{
    public const string CodeIdRequired = $"{nameof(Transaction)}.{nameof(Transaction.Id)}";
    public const string CodeDateRequired = $"{nameof(Transaction)}.{nameof(Transaction.Date)}";
    public const string CodeAccountIdRequired = $"{nameof(Transaction)}.{nameof(Transaction.AccountId)}";
    public const string CodeCategoryIdRequired = $"{nameof(Transaction)}.{nameof(Transaction.CategoryId)}";
    public const string CodeUserIdRequired = $"{nameof(Transaction)}.{nameof(Transaction.UserId)}";
    public const string CodeCashflowRequired = $"{nameof(Transaction)}.{nameof(Transaction.Cashflow)}";
    public const string CodeFrequencyInvalid = $"{nameof(Transaction)}.{nameof(FrequencyInvalid)}";
    public const string CodeRecurrenceInvalid = $"{nameof(Transaction)}.{nameof(RecurrenceInvalid)}";
    public const string CodeNotFound = $"{nameof(Transaction)}.{nameof(NotFound)}";
    public const string CodeAccountNotFound = $"{nameof(Transaction)}.{nameof(AccountNotFound)}";
    public const string CodeCategoryNotFound = $"{nameof(Transaction)}.{nameof(CategoryNotFound)}";
    public const string CodeCategoryNameNotFound = $"{nameof(Transaction)}.{nameof(CategoryNameNotFound)}";

    public static ResultError IdRequired => new(CodeIdRequired, "Id is required");
    public static ResultError DateRequired => new(CodeDateRequired, "Date is required");
    public static ResultError FrequencyInvalid => new(CodeFrequencyInvalid, "Frequency must be positive");
    public static ResultError RecurrenceInvalid => new(CodeRecurrenceInvalid, "Recurrence cannot be negative");
    public static ResultError AccountIdRequired => new(CodeAccountIdRequired, "AccountId is required");
    public static ResultError CategoryIdRequired => new(CodeCategoryIdRequired, "CategoryId is required");
    public static ResultError UserIdRequired => new(CodeUserIdRequired, "UserId is required");
    public static ResultError CashflowRequired => new(CodeCashflowRequired, "CashflowId and CashflowDate are required");
    public static ResultError NotFound(TransactionId id) => new(CodeNotFound, $"Transaction '{(Guid)id}' not found");
    public static ResultError AccountNotFound(AccountId id) => new(CodeAccountNotFound, $"Account '{(Guid)id}' does not exists.");
    public static ResultError CategoryNotFound(CategoryId id) => new(CodeCategoryNotFound, $"Category '{(Guid)id}' does not exists.");
    public static ResultError CategoryNameNotFound(string name) => new(CodeCategoryNameNotFound, $"Category '{name}' not found");
}
