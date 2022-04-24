using System.Collections.Immutable;
using System.Globalization;

using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Domain.Features.Transactions;

public record Transaction : IAggregateRoot
{
    private readonly decimal _amount;
    private readonly DateTime _date;
    private readonly Guid _id;
    private readonly Guid _userId;

    public Transaction()
    {
        Tags = ImmutableList<string>.Empty;
    }

    public Guid Id
    {
        get => _id;
        init
        {
            if (value.Equals(default))
            {
                throw new TransactionDomainException($"{nameof(Id)} is required");
            }

            _id = value;
        }
    }

    public DateTime Date
    {
        get => _date;
        init
        {
            if (value.Equals(default))
            {
                throw new TransactionDomainException($"{nameof(Date)} is required");
            }

            _date = value;
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

    public Guid? CashflowId { get; init; }

    public DateTime? CashflowDate { get; init; }

    public IReadOnlyList<string> Tags { get; private init; }

    public Guid UserId
    {
        get => _userId;
        init
        {
            if (value.Equals(default))
            {
                throw new TransactionDomainException($"{nameof(UserId)} is required");
            }

            _userId = value;
        }
    }

    public static Transaction Create(DateTime date, decimal amount, string description, Guid categoryId,
        Guid accountId, Guid userId)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Date = date,
            Amount = amount,
            Description = description,
            CategoryId = categoryId,
            AccountId = accountId,
            UserId = userId
        };
    }

    public static Transaction Create(DateTime date, decimal amount, string description, Guid categoryId,
        Guid accountId, Guid cashflowId, DateTime cashflowDate, Guid userId)
    {
        if (cashflowId.Equals(default))
        {
            throw new TransactionDomainException($"{nameof(CashflowId)} is required");
        }

        if (cashflowDate.Equals(default))
        {
            throw new TransactionDomainException($"{nameof(CashflowDate)} is required");
        }

        return new Transaction
        {
            Id = Guid.NewGuid(),
            Date = date,
            Amount = amount,
            Description = description,
            CategoryId = categoryId,
            AccountId = accountId,
            CashflowId = cashflowId,
            CashflowDate = cashflowDate,
            UserId = userId
        };
    }

    public Transaction AddTags(params string[] tags)
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
