using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Cashflows.Core;

internal sealed record Cashflow(
    Id Id,
    DateOnly EffectiveDate,
    Money Amount,
    string IntervalType,
    int Frequency,
    int Recurrence,
    string Description,
    Id AccountId,
    Id CategoryId,
    bool Inactive,
    string Tags)
{
    public static Cashflow Valid(
        Id id, DateOnly effectiveDate, Money amount, string intervalType, int frequency, int recurrence, string description,
        Id accountId, Id categoryId, bool inactive, string tags) =>
        new(id, effectiveDate, amount, intervalType, frequency, recurrence, description, accountId, categoryId, inactive, tags);

    public Payment Pay(Id paymentId, DateOnly date, Money amount, DateOnly cashflowDate) =>
        new(paymentId, date, amount, Description, AccountId, CategoryId, Id, cashflowDate, Tags);

    public Result<Cashflow> Modify(Money amount, DateOnly effectiveDate, string? description, Tags tags) =>
        Inactive
            ? CashflowErrors.Cancelled
            : this with { Amount = amount, EffectiveDate = effectiveDate, Description = description ?? Description, Tags = tags.Text };

    public Result<Cashflow> Cancel() => Inactive ? CashflowErrors.AlreadyInactive : this with { Inactive = true };
}

internal static class CashflowErrors
{
    public const string CodeNotFound = "cashflow-not-found";
    public const string CodeAlreadyInactive = "cashflow-already-inactive";
    public const string CodeCancelled = "cashflow-cancelled";

    public static NotFoundError NotFound => new(CodeNotFound, "Cashflow was not found.");

    public static ResultError AlreadyInactive => new(CodeAlreadyInactive, "Cashflow is already inactive.");

    public static ResultError Cancelled => new(CodeCancelled, "A cancelled cashflow cannot be changed.");
}
