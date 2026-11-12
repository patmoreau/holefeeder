using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Sync.Core;

internal static class SyncRecordErrors
{
    public const string CodeAccountNotFound = "sync-account-not-found";
    public const string CodeCategoryNotFound = "sync-category-not-found";
    public const string CodeStoreItemNotFound = "sync-store-item-not-found";
    public const string CodeTransactionNotFound = "sync-transaction-not-found";
    public const string CodeCashflowNotFound = "sync-cashflow-not-found";
    public const string CodeAccountNameInvalid = "sync-account-name-invalid";
    public const string CodeAccountOpenDateRequired = "sync-account-open-date-required";
    public const string CodeCategoryNameInvalid = "sync-category-name-invalid";
    public const string CodeStoreItemCodeRequired = "sync-store-item-code-required";
    public const string CodeTransactionDateRequired = "sync-transaction-date-required";
    public const string CodeTransactionAccountRequired = "sync-transaction-account-required";
    public const string CodeTransactionCategoryRequired = "sync-transaction-category-required";
    public const string CodeCashflowEffectiveDateRequired = "sync-cashflow-effective-date-required";
    public const string CodeCashflowFrequencyInvalid = "sync-cashflow-frequency-invalid";
    public const string CodeCashflowRecurrenceInvalid = "sync-cashflow-recurrence-invalid";
    public const string CodeCashflowAccountRequired = "sync-cashflow-account-required";
    public const string CodeCashflowCategoryRequired = "sync-cashflow-category-required";

    public static ResultError AccountNotFound => new NotFoundError(CodeAccountNotFound, "Account was not found.");
    public static ResultError CategoryNotFound => new NotFoundError(CodeCategoryNotFound, "Category was not found.");
    public static ResultError StoreItemNotFound => new NotFoundError(CodeStoreItemNotFound, "Store item was not found.");
    public static ResultError TransactionNotFound => new NotFoundError(CodeTransactionNotFound, "Transaction was not found.");
    public static ResultError CashflowNotFound => new NotFoundError(CodeCashflowNotFound, "Cashflow was not found.");

    public static ResultError AccountNameInvalid =>
        new(CodeAccountNameInvalid, "Account name is required and must be at most 100 characters.");
    public static ResultError AccountOpenDateRequired => new(CodeAccountOpenDateRequired, "Account open date is required.");
    public static ResultError CategoryNameInvalid =>
        new(CodeCategoryNameInvalid, "Category name is required and must be at most 255 characters.");
    public static ResultError StoreItemCodeRequired => new(CodeStoreItemCodeRequired, "Store item code is required.");
    public static ResultError TransactionDateRequired => new(CodeTransactionDateRequired, "Transaction date is required.");
    public static ResultError TransactionAccountRequired => new(CodeTransactionAccountRequired, "Transaction account is required.");
    public static ResultError TransactionCategoryRequired => new(CodeTransactionCategoryRequired, "Transaction category is required.");
    public static ResultError CashflowEffectiveDateRequired =>
        new(CodeCashflowEffectiveDateRequired, "Cashflow effective date is required.");
    public static ResultError CashflowFrequencyInvalid => new(CodeCashflowFrequencyInvalid, "Cashflow frequency must be more than zero.");
    public static ResultError CashflowRecurrenceInvalid => new(CodeCashflowRecurrenceInvalid, "Cashflow recurrence must be zero or more.");
    public static ResultError CashflowAccountRequired => new(CodeCashflowAccountRequired, "Cashflow account is required.");
    public static ResultError CashflowCategoryRequired => new(CodeCashflowCategoryRequired, "Cashflow category is required.");

    public static ResultError NotFound(SyncTable table) =>
        table switch
        {
            SyncTable.Accounts => AccountNotFound,
            SyncTable.Categories => CategoryNotFound,
            SyncTable.StoreItems => StoreItemNotFound,
            SyncTable.Transactions => TransactionNotFound,
            _ => CashflowNotFound
        };

    public static Result<ISyncRecord> Validated(ISyncRecord record, params (bool IsValid, ResultError Error)[] rules)
    {
        var validation = ResultAggregate.Create();
        foreach (var (isValid, error) in rules.Where(rule => !rule.IsValid))
        {
            validation.AddResult(error);
        }

        return validation.OnSuccess(record.ToResult);
    }
}
