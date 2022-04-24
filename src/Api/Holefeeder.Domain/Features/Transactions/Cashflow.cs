using System.Collections.Immutable;
using System.Globalization;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Domain.Features.Transactions;

public record Cashflow : IAggregateRoot
{
    private readonly decimal _amount;
    private readonly int _frequency;
    private readonly int _recurrence;

    public Cashflow(Guid id, DateTime effectiveDate, decimal amount, Guid userId)
    {
        if (id.Equals(default))
        {
            throw new TransactionDomainException($"{nameof(Id)} is required");
        }

        Id = id;

        if (effectiveDate.Equals(default))
        {
            throw new TransactionDomainException($"{nameof(EffectiveDate)} is required");
        }

        EffectiveDate = effectiveDate;

        Amount = amount;

        if (userId.Equals(default))
        {
            throw new TransactionDomainException($"{nameof(UserId)} is required");
        }

        UserId = userId;

        Tags = ImmutableList<string>.Empty;
    }

    public Guid Id { get; }

    public DateTime EffectiveDate { get; init; }

    public DateIntervalType IntervalType { get; init; } = DateIntervalType.Weekly;

    public int Frequency
    {
        get => _frequency;
        init
        {
            if (value <= 0)
            {
                throw new TransactionDomainException($"{nameof(Frequency)} must be positive");
            }

            _frequency = value;
        }
    }

    public int Recurrence
    {
        get => _recurrence;
        init
        {
            if (value < 0)
            {
                throw new TransactionDomainException($"{nameof(Recurrence)} cannot be negative");
            }

            _recurrence = value;
        }
    }

    public decimal Amount
    {
        get => _amount;
        init
        {
            if (value < 0m)
            {
                throw new TransactionDomainException($"{nameof(Amount)} cannot be negative");
            }

            _amount = value;
        }
    }

    public string Description { get; init; } = string.Empty;

    public Guid AccountId { get; init; }

    public Guid CategoryId { get; init; }

    public IReadOnlyList<string> Tags { get; private init; }

    public bool Inactive { get; init; }

    public Guid UserId { get; init; }

    public static Cashflow Create(DateTime effectiveDate, DateIntervalType intervalType, int frequency,
        int recurrence, decimal amount, string description, Guid categoryId, Guid accountId, Guid userId)
    {
        return new(Guid.NewGuid(), effectiveDate, amount, userId)
        {
            IntervalType = intervalType,
            Frequency = frequency,
            Recurrence = recurrence,
            Description = description,
            CategoryId = categoryId,
            AccountId = accountId
        };
    }

    public Cashflow AddTags(params string[] tags)
    {
        var newTags = tags.Where(t => !string.IsNullOrWhiteSpace(t) &&
                                      !Tags.Contains(t,
                                          StringComparer.Create(CultureInfo.InvariantCulture,
                                              CompareOptions.IgnoreCase)))
            .ToList();

        if (!newTags.Any())
        {
            return this;
        }

        return this with {Tags = newTags.ToImmutableArray()};
    }
}
