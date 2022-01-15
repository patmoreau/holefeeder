using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;

public record Cashflow : IAggregateRoot
{
    private readonly int _frequency;
    private readonly int _recurrence;
    private readonly decimal _amount;

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
                throw HolefeederDomainException.Create<Cashflow>($"{nameof(Frequency)} must be positive");
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
                throw HolefeederDomainException.Create<Cashflow>($"{nameof(Recurrence)} cannot be negative");
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
                throw HolefeederDomainException.Create<Cashflow>($"{nameof(Amount)} cannot be negative");
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

    public Cashflow(Guid id, DateTime effectiveDate, decimal amount, Guid userId)
    {
        if (id.Equals(default))
        {
            throw HolefeederDomainException.Create<Cashflow>($"{nameof(Id)} is required");
        }

        Id = id;

        if (effectiveDate.Equals(default))
        {
            throw HolefeederDomainException.Create<Cashflow>($"{nameof(EffectiveDate)} is required");
        }

        EffectiveDate = effectiveDate;

        Amount = amount;

        if (userId.Equals(default))
        {
            throw HolefeederDomainException.Create<Cashflow>($"{nameof(UserId)} is required");
        }

        UserId = userId;

        Tags = ImmutableList<string>.Empty;
    }

    public static Cashflow Create(DateTime effectiveDate, DateIntervalType intervalType, int frequency,
        int recurrence, decimal amount, string description, Guid categoryId, Guid accountId, Guid userId)
        => new(Guid.NewGuid(), effectiveDate, amount, userId)
        {
            IntervalType = intervalType,
            Frequency = frequency,
            Recurrence = recurrence,
            Description = description,
            CategoryId = categoryId,
            AccountId = accountId,
        };

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
