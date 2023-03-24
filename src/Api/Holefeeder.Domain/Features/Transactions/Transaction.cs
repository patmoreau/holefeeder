using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Domain.Features.Transactions;

public record Transaction : Entity, IAggregateRoot
{
    private readonly Guid _accountId;
    private readonly decimal _amount;
    private readonly Guid _categoryId;
    private readonly DateTime _date;
    private readonly Guid _id;
    private readonly Guid _userId;

    public sealed override required Guid Id
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

    public required DateTime Date
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

    public required decimal Amount
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

    public required Guid AccountId
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

    public Account? Account { get; init; }

    public required Guid CategoryId
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

    public Category? Category { get; init; }

    public Guid? CashflowId { get; private set; }
    public Cashflow? Cashflow { get; init; }

    public DateTime? CashflowDate { get; private set; }

    public IReadOnlyCollection<string> Tags { get; private set; } = ImmutableList<string>.Empty;

    public required Guid UserId
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
        Guid categoryId, Guid userId) =>
        new()
        {
            Id = id,
            Date = date,
            Amount = amount,
            AccountId = accountId,
            CategoryId = categoryId,
            UserId = userId,
            Description = description
        };

    public static Transaction Create(DateTime date, decimal amount, string description, Guid accountId, Guid categoryId,
        Guid userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            Date = date,
            Amount = amount,
            AccountId = accountId,
            CategoryId = categoryId,
            UserId = userId,
            Description = description
        };

    public Transaction SetTags(params string[] tags)
    {
        List<string> newTags = tags.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList();

        Tags = newTags.ToImmutableArray();
        return this;
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

        CashflowId = cashflowId;
        CashflowDate = cashflowDate;
        return this;
    }
}
