using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext
{
    public record Cashflow : IAggregateRoot
    {
        private readonly Guid _id;
        private readonly DateTime _effectiveDate;
        private readonly int _frequency;
        private readonly int _recurrence;
        private readonly decimal _amount;
        private readonly Guid _userId;

        public Guid Id
        {
            get => _id;
            init
            {
                if (value.Equals(default))
                {
                    throw new HolefeederDomainException($"{nameof(Id)} is required");
                }

                _id = value;
            }
        }

        public DateTime EffectiveDate
        {
            get => _effectiveDate;
            init
            {
                if (value.Equals(default))
                {
                    throw new HolefeederDomainException($"{nameof(EffectiveDate)} is required");
                }

                _effectiveDate = value;
            }
        }

        public DateIntervalType IntervalType { get; init; }

        public int Frequency
        {
            get => _frequency;
            init
            {
                if (value > 0)
                {
                    throw new HolefeederDomainException($"{nameof(Frequency)} must be positive");
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
                    throw new HolefeederDomainException($"{nameof(Recurrence)} cannot be negative");
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
                    throw new HolefeederDomainException($"{nameof(Amount)} cannot be negative");
                }

                _amount = value;
            }
        }

        public string Description { get; init; }

        public Guid AccountId { get; init; }

        public Guid CategoryId { get; init; }

        public IReadOnlyCollection<string> Tags { get; private init; }

        public bool Inactive { get; init; }

        public Guid UserId
        {
            get => _userId;
            init
            {
                if (value.Equals(default))
                {
                    throw new HolefeederDomainException($"{nameof(UserId)} is required");
                }

                _userId = value;
            }
        }

        private Cashflow()
        {
            Tags = ImmutableList<string>.Empty;
        }

        public static Cashflow Create(DateTime effectiveDate, DateIntervalType intervalType, int frequency,
            int recurrence, decimal amount, string description, Guid categoryId, Guid accountId, Guid userId)
            => new()
            {
                Id = Guid.NewGuid(),
                EffectiveDate = effectiveDate,
                IntervalType = intervalType,
                Frequency = frequency,
                Recurrence = recurrence,
                Amount = amount,
                Description = description,
                CategoryId = categoryId,
                AccountId = accountId,
                UserId = userId
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

            return this with { Tags = newTags.ToImmutableList() };
        }
    }
}
