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

    public static ResultError AccountNotFound => new NotFoundError(CodeAccountNotFound);
    public static ResultError CategoryNotFound => new NotFoundError(CodeCategoryNotFound);
    public static ResultError StoreItemNotFound => new NotFoundError(CodeStoreItemNotFound);
    public static ResultError TransactionNotFound => new NotFoundError(CodeTransactionNotFound);
    public static ResultError CashflowNotFound => new NotFoundError(CodeCashflowNotFound);

    public static ResultError AccountNameInvalid =>
        new(CodeAccountNameInvalid);
    public static ResultError AccountOpenDateRequired => new(CodeAccountOpenDateRequired);
    public static ResultError CategoryNameInvalid =>
        new(CodeCategoryNameInvalid);
    public static ResultError StoreItemCodeRequired => new(CodeStoreItemCodeRequired);
    public static ResultError TransactionDateRequired => new(CodeTransactionDateRequired);
    public static ResultError TransactionAccountRequired => new(CodeTransactionAccountRequired);
    public static ResultError TransactionCategoryRequired => new(CodeTransactionCategoryRequired);
    public static ResultError CashflowEffectiveDateRequired =>
        new(CodeCashflowEffectiveDateRequired);
    public static ResultError CashflowFrequencyInvalid => new(CodeCashflowFrequencyInvalid);
    public static ResultError CashflowRecurrenceInvalid => new(CodeCashflowRecurrenceInvalid);
    public static ResultError CashflowAccountRequired => new(CodeCashflowAccountRequired);
    public static ResultError CashflowCategoryRequired => new(CodeCashflowCategoryRequired);

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
