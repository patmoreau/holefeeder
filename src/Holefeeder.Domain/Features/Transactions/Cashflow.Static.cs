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
        var result = Result.Validate(EffectiveDateValidation(effectiveDate), FrequencyValidation(frequency),
            RecurrenceValidation(recurrence), AccountIdValidation(accountId), CategoryIdValidation(categoryId),
            UserIdValidation(userId));
        if (result.IsFailure)
        {
            return Result<Cashflow>.Failure(result.Error);
        }

        return Result<Cashflow>.Success(
            new Cashflow(CashflowId.New, effectiveDate, intervalType, accountId, categoryId, userId)
            {
                Amount = amount,
                IntervalType = intervalType,
                Frequency = frequency,
                Recurrence = recurrence,
                Description = description,
            });
    }

    public static Result<Cashflow> Import(CashflowId id, DateOnly effectiveDate, DateIntervalType intervalType,
        int frequency, int recurrence, Money amount, string description, CategoryId categoryId, AccountId accountId,
        bool inactive, UserId userId)
    {
        var result = Result.Validate(IdValidation(id), EffectiveDateValidation(effectiveDate),
            FrequencyValidation(frequency), RecurrenceValidation(recurrence), AccountIdValidation(accountId),
            CategoryIdValidation(categoryId), UserIdValidation(userId));
        if (result.IsFailure)
        {
            return Result<Cashflow>.Failure(result.Error);
        }

        return Result<Cashflow>.Success(
            new Cashflow(id, effectiveDate, intervalType, accountId, categoryId, userId)
            {
                Amount = amount,
                IntervalType = intervalType,
                Frequency = frequency,
                Recurrence = recurrence,
                Description = description,
            });
    }

    private bool IsUnpaid(DateOnly effectiveDate, DateOnly nextDate) =>
        LastPaidDate is null
            ? nextDate >= effectiveDate
            : nextDate > LastPaidDate && nextDate > LastCashflowDate;
}
