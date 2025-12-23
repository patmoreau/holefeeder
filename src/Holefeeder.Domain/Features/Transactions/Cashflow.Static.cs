using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Transactions;

public partial record Cashflow
{
    public static Result<Cashflow> Create(DateOnly effectiveDate, DateIntervalType intervalType, int frequency,
        int recurrence,
        Money amount, string description, CategoryId categoryId, AccountId accountId, UserId userId)
    {
        var result = ResultAggregate.Create()
            .Ensure(EffectiveDateValidation(effectiveDate))
            .Ensure(FrequencyValidation(frequency))
            .Ensure(RecurrenceValidation(recurrence))
            .Ensure(AccountIdValidation(accountId))
            .Ensure(CategoryIdValidation(categoryId))
            .Ensure(UserIdValidation(userId));

        return result.OnSuccess(
            () => new Cashflow(CashflowId.New, effectiveDate, intervalType, accountId, categoryId, userId)
            {
                Amount = amount,
                IntervalType = intervalType,
                Frequency = frequency,
                Recurrence = recurrence,
                Description = description,
            }.ToResult());
    }

    public static Result<Cashflow> Import(CashflowId id, DateOnly effectiveDate, DateIntervalType intervalType,
        int frequency, int recurrence, Money amount, string description, CategoryId categoryId, AccountId accountId,
        bool inactive, UserId userId)
    {
        var result = ResultAggregate.Create()
            .Ensure(IdValidation(id))
            .Ensure(EffectiveDateValidation(effectiveDate))
            .Ensure(FrequencyValidation(frequency))
            .Ensure(RecurrenceValidation(recurrence))
            .Ensure(AccountIdValidation(accountId))
            .Ensure(CategoryIdValidation(categoryId))
            .Ensure(UserIdValidation(userId));

        return result.OnSuccess(
            () => new Cashflow(id, effectiveDate, intervalType, accountId, categoryId, userId)
            {
                Amount = amount,
                IntervalType = intervalType,
                Frequency = frequency,
                Recurrence = recurrence,
                Description = description,
                Inactive = inactive
            }.ToResult());
    }

    public static decimal CalculateUpcomingVariation(DateOnly targetDate, IReadOnlyCollection<Cashflow> cashflows) =>
        cashflows
            .Select(c => c.GetUpcoming(targetDate).Count() * c.Amount * c.Category!.Type.Multiplier)
            .Sum();
}
