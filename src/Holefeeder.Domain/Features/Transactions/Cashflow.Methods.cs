using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Transactions;

public partial record Cashflow
{
    public Result<Cashflow> Cancel() => Inactive
        ? Result<Cashflow>.Failure(CashflowErrors.AlreadyInactive(Id))
        : Result<Cashflow>.Success(this with { Inactive = true });

    public Result<Cashflow> SetTags(params string[] tags)
    {
        var newTags = tags.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().Select(x => x.ToLowerInvariant())
            .ToList();
        _tags = newTags.ToImmutableList();
        return Result<Cashflow>.Success(this);
    }

    public IEnumerable<DateOnly> GetUpcoming(DateOnly to)
    {
        List<DateOnly> dates = [];

        if (Inactive)
        {
            return dates;
        }

        dates.AddRange(IntervalType
            .DatesInRange(EffectiveDate, EffectiveDate, to, Frequency)
            .Where(futureDate => IsUnpaid(EffectiveDate, futureDate)));

        return dates;
    }

    public Result<Cashflow> Modify(DateOnly? effectiveDate = null, DateIntervalType? intervalType = null,
        int? frequency = null, int? recurrence = null, Money? amount = null, string? description = null,
        CategoryId? categoryId = null, AccountId? accountId = null, bool? inactive = null)
    {
        var newEffectiveDate = effectiveDate ?? EffectiveDate;
        var newIntervalType = intervalType ?? IntervalType;
        var newFrequency = frequency ?? Frequency;
        var newRecurrence = recurrence ?? Recurrence;
        var newAmount = amount ?? Amount;
        var newDescription = description ?? Description;
        var newCategoryId = categoryId ?? CategoryId;
        var newAccountId = accountId ?? AccountId;
        var newInactive = inactive ?? Inactive;

        var result = Result.Validate(EffectiveDateValidation(newEffectiveDate),
            FrequencyValidation(newFrequency), RecurrenceValidation(newRecurrence), AccountIdValidation(newAccountId),
            CategoryIdValidation(newCategoryId));
        if (result.IsFailure)
        {
            return Result<Cashflow>.Failure(result.Error);
        }

        return Result<Cashflow>.Success(
            this with
            {
                EffectiveDate = newEffectiveDate,
                IntervalType = newIntervalType,
                Frequency = newFrequency,
                Recurrence = newRecurrence,
                Amount = newAmount,
                Description = newDescription,
                CategoryId = newCategoryId,
                AccountId = newAccountId,
                Inactive = newInactive
            });
    }
}
