using System.Collections.Immutable;

using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Domain.Features.Transactions;

public record Transaction : IAggregateRoot
{
    private readonly Guid _id;
    private readonly DateTime _date;
    private readonly decimal _amount;
    private readonly Guid _accountId;
    private readonly Guid _categoryId;
    private readonly Guid _userId;

    private Transaction(Guid id, DateTime date, decimal amount, Guid accountId, Guid categoryId, Guid userId)
    {
        Id = id;
        Date = date;
        Amount = amount;
        AccountId = accountId;
        CategoryId = categoryId;
        UserId = userId;
    }

    public Guid Id
    {
        get => _id;
        init
        {
            if (value.Equals(Guid.Empty))
            {
                throw new TransactionDomainException($"{nameof(Id)} is required", nameof(Transaction));
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
                throw new TransactionDomainException($"{nameof(Date)} is required", nameof(Transaction));
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
                throw new TransactionDomainException($"{nameof(Amount)} cannot be negative", nameof(Transaction));
            }

            _amount = value;
        }
    }

    public string Description { get; init; } = string.Empty;

    public Guid AccountId
    {
        get => _accountId;
        init
        {
            if (value.Equals(Guid.Empty))
            {
                throw new TransactionDomainException($"{nameof(AccountId)} is required", nameof(Transaction));
            }

            _accountId = value;
        }
    }

    public Guid CategoryId
    {
        get => _categoryId;
        init
        {
            if (value.Equals(Guid.Empty))
            {
                throw new TransactionDomainException($"{nameof(CategoryId)} is required", nameof(Transaction));
            }

            _categoryId = value;
        }
    }

    public Guid? CashflowId { get; private init; }

    public DateTime? CashflowDate { get; private init; }

    public IReadOnlyList<string> Tags { get; private init; } = ImmutableList<string>.Empty;

    public Guid UserId
    {
        get => _userId;
        init
        {
            if (value.Equals(Guid.Empty))
            {
                throw new TransactionDomainException($"{nameof(UserId)} is required", nameof(Transaction));
            }

            _userId = value;
        }
    }

    public static Transaction Create(Guid id, DateTime date, decimal amount, string description, Guid accountId,
        Guid categoryId, Guid userId)
    {
        return new(id, date, amount, accountId, categoryId, userId) {Description = description};
    }

    public static Transaction Create(DateTime date, decimal amount, string description, Guid accountId, Guid categoryId,
        Guid userId)
    {
        return new(Guid.NewGuid(), date, amount, accountId, categoryId, userId) {Description = description};
    }

    public Transaction SetTags(params string[] tags)
    {
        var newTags = tags.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList();

        return this with {Tags = newTags.ToImmutableArray()};
    }

    public Transaction ApplyCashflow(Guid cashflowId, DateTime cashflowDate)
    {
        if (cashflowId.Equals(Guid.Empty))
        {
            throw new TransactionDomainException($"{nameof(CashflowId)} is required", nameof(Transaction));
        }

        if (cashflowDate.Equals(default))
        {
            throw new TransactionDomainException($"{nameof(CashflowDate)} is required", nameof(Transaction));
        }

        return this with {CashflowId = cashflowId, CashflowDate = cashflowDate};
    }
}
