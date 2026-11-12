using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.MyData.Core;

internal static class MyDataRecordErrors
{
    public const string CodeAccountNameInvalid = "my-data-account-name-invalid";
    public const string CodeAccountOpenDateRequired = "my-data-account-open-date-required";
    public const string CodeCategoryNameInvalid = "my-data-category-name-invalid";
    public const string CodeCashflowEffectiveDateRequired = "my-data-cashflow-effective-date-required";
    public const string CodeCashflowFrequencyInvalid = "my-data-cashflow-frequency-invalid";
    public const string CodeCashflowRecurrenceInvalid = "my-data-cashflow-recurrence-invalid";
    public const string CodeTransactionDateRequired = "my-data-transaction-date-required";

    public static ResultError AccountNameInvalid =>
        new(CodeAccountNameInvalid, "Account name is required and must be at most 100 characters.");
    public static ResultError AccountOpenDateRequired => new(CodeAccountOpenDateRequired, "Account open date is required.");
    public static ResultError CategoryNameInvalid =>
        new(CodeCategoryNameInvalid, "Category name is required and must be at most 255 characters.");
    public static ResultError CashflowEffectiveDateRequired =>
        new(CodeCashflowEffectiveDateRequired, "Cashflow effective date is required.");
    public static ResultError CashflowFrequencyInvalid => new(CodeCashflowFrequencyInvalid, "Cashflow frequency must be more than zero.");
    public static ResultError CashflowRecurrenceInvalid => new(CodeCashflowRecurrenceInvalid, "Cashflow recurrence cannot be negative.");
    public static ResultError TransactionDateRequired => new(CodeTransactionDateRequired, "Transaction date is required.");

    public static Result<T> Validated<T>(T record, params (bool IsValid, ResultError Error)[] rules) =>
        rules.FirstOrDefault(rule => !rule.IsValid) is { Error: { } error } ? error : record;
}
