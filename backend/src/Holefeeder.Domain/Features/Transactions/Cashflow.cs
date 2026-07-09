using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Transactions;

public partial record Cashflow : IAggregateRoot
{
    private IReadOnlyCollection<string> _tags = [];

    private Cashflow(CashflowId id, DateOnly effectiveDate, DateIntervalType intervalType, AccountId accountId,
        CategoryId categoryId, UserId userId)
    {
        Id = id;
        EffectiveDate = effectiveDate;
        IntervalType = intervalType;
        AccountId = accountId;
        CategoryId = categoryId;
        UserId = userId;
    }

    public CashflowId Id { get; }

    public DateOnly EffectiveDate { get; private init; }

    public DateIntervalType IntervalType { get; private init; }

    public int Frequency { get; private init; }

    public int Recurrence { get; private init; }

    public Money Amount { get; private init; }

    public string Description { get; private init; } = string.Empty;

    public AccountId AccountId { get; private init; }

    public Account? Account { get; private init; }

    public CategoryId CategoryId { get; private init; }

    public Category? Category { get; init; }

    public IReadOnlyCollection<string> Tags => _tags.ToImmutableList();

    public IReadOnlyCollection<Transaction> Transactions { get; } = new List<Transaction>();

    public DateOnly? LastPaidDate => Transactions.Count > 0 ? Transactions.Max(x => x.Date) : null;

    public DateOnly? LastCashflowDate => Transactions.Max(x => x.CashflowDate);

    public bool Inactive { get; private init; }

    public UserId UserId { get; private init; }
}

public sealed record CashflowId : StronglyTypedId<CashflowId>;
