using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Transactions;

public partial record Transaction : IAggregateRoot
{
    private IReadOnlyCollection<string> _tags = [];

    private Transaction(TransactionId id, DateOnly date, Money amount, AccountId accountId, CategoryId categoryId, UserId userId)
    {
        Id = id;
        Date = date;
        Amount = amount;
        AccountId = accountId;
        CategoryId = categoryId;
        UserId = userId;
    }

    public TransactionId Id { get; private init; }

    public DateOnly Date { get; private init; }

    public Money Amount { get; private init; }

    public string Description { get; init; } = string.Empty;

    public AccountId AccountId { get; init; }

    public Account? Account { get; init; }

    public CategoryId CategoryId { get; init; }

    public Category? Category { get; init; }

    public CashflowId? CashflowId { get; private init; }

    public Cashflow? Cashflow { get; init; }

    public DateOnly? CashflowDate { get; private init; }

    public IReadOnlyCollection<string> Tags => _tags.ToImmutableList();

    public UserId UserId { get; init; }
}

public sealed record TransactionId : StronglyTypedId<TransactionId>;
