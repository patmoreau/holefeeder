namespace Holefeeder.Domain.Features.Transactions;

public static class CashflowErrors
{
    public const string CodeIdRequired = $"{nameof(Cashflow)}.{nameof(IdRequired)}";
    public const string CodeEffectiveDateRequired = $"{nameof(Cashflow)}.{nameof(EffectiveDateRequired)}";
    public const string CodeFrequencyInvalid = $"{nameof(Cashflow)}.{nameof(FrequencyInvalid)}";
    public const string CodeRecurrenceInvalid = $"{nameof(Cashflow)}.{nameof(RecurrenceInvalid)}";
    public const string CodeAccountIdRequired = $"{nameof(Cashflow)}.{nameof(AccountIdRequired)}";
    public const string CodeCategoryIdRequired = $"{nameof(Cashflow)}.{nameof(CategoryIdRequired)}";
    public const string CodeUserIdRequired = $"{nameof(Cashflow)}.{nameof(UserIdRequired)}";
    public const string CodeAlreadyInactive = $"{nameof(Cashflow)}.{nameof(AlreadyInactive)}";
    public const string CodeNotFound = $"{nameof(Cashflow)}.{nameof(NotFound)}";

    public static ResultError IdRequired => new(CodeIdRequired, "Id is required");
    public static ResultError EffectiveDateRequired => new(CodeEffectiveDateRequired, "EffectiveDate is required");
    public static ResultError FrequencyInvalid => new(CodeFrequencyInvalid, "Frequency must be positive");
    public static ResultError RecurrenceInvalid => new(CodeRecurrenceInvalid, "Recurrence cannot be negative");
    public static ResultError AccountIdRequired => new(CodeAccountIdRequired, "AccountId is required");
    public static ResultError CategoryIdRequired => new(CodeCategoryIdRequired, "CategoryId is required");
    public static ResultError UserIdRequired => new(CodeUserIdRequired, "UserId is required");
    public static ResultError AlreadyInactive(CashflowId id) => new(CodeAlreadyInactive, $"Cashflow {id} already inactive");
    public static ResultError NotFound(CashflowId id) => new(CodeNotFound, $"Cashflow '{id}' not found");
}
