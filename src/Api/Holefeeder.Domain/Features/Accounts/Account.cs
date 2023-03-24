using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Domain.Features.Accounts;

public sealed record Account : Entity, IAggregateRoot
{
    private readonly Guid _id;
    private readonly string _name = string.Empty;
    private readonly DateTime _openDate;
    private readonly Guid _userId;

    public override required Guid Id
    {
        get => _id;
        init
        {
            if (value.Equals(default))
            {
                throw new AccountDomainException($"{nameof(Id)} is required");
            }

            _id = value;
        }
    }

    public required AccountType Type { get; init; }

    public required string Name
    {
        get => _name;
        init
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 100)
            {
                throw new AccountDomainException($"{nameof(Name)} must be from 1 to 100 characters");
            }

            _name = value;
        }
    }

    public bool Favorite { get; init; }

    public decimal OpenBalance { get; init; }

    public required DateTime OpenDate
    {
        get => _openDate;
        init
        {
            if (value.Equals(default))
            {
                throw new AccountDomainException($"{nameof(OpenDate)} is required");
            }

            _openDate = value;
        }
    }

    public string Description { get; init; } = string.Empty;

    public bool Inactive { get; init; }

    public required Guid UserId
    {
        get => _userId;
        init
        {
            if (value.Equals(default))
            {
                throw new AccountDomainException($"{nameof(UserId)} is required");
            }

            _userId = value;
        }
    }

    public IReadOnlyCollection<Cashflow> Cashflows { get; init; } = new List<Cashflow>();
    public IReadOnlyCollection<Transaction> Transactions { get; init; } = new List<Transaction>();

    public static Account Create(AccountType type, string name, decimal openBalance, DateTime openDate,
        string description, Guid userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            Type = type,
            Name = name,
            OpenDate = openDate,
            UserId = userId,
            OpenBalance = openBalance,
            Description = description
        };

    public Account Close()
    {
        if (Inactive)
        {
            throw new AccountDomainException("Account already closed");
        }

        if (Cashflows.Any())
        {
            throw new AccountDomainException("Account has active cashflows");
        }

        return this with { Inactive = true };
    }

    public decimal CalculateBalance() =>
        OpenBalance + Transactions.Sum(t => t.Amount * t.Category!.Type.Multiplier * Type.Multiplier);

    public DateTime CalculateLastTransactionDate() => Transactions.Any() ? Transactions.Max(t => t.Date) : OpenDate;
}
