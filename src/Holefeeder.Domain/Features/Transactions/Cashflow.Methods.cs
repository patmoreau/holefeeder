using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Transactions;

public partial record Cashflow
{
    public Result<Cashflow> Cancel() => Inactive
        ? CashflowErrors.AlreadyInactive(Id)
        : this with {Inactive = true};

    public Result<Cashflow> SetTags(params string[] tags)
    {
        var newTags = tags.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().Select(x => x.ToLowerInvariant())
            .ToList();
        _tags = newTags.ToImmutableList();
        return this;
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

        var result = ResultAggregate.Create()
            .Ensure(EffectiveDateValidation(newEffectiveDate))
            .Ensure(FrequencyValidation(newFrequency))
            .Ensure(RecurrenceValidation(newRecurrence))
            .Ensure(AccountIdValidation(newAccountId))
            .Ensure(CategoryIdValidation(newCategoryId));

        return result.OnSuccess(
            () => (this with
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
            }).ToResult());
    }
}
