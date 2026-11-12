using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Application.Features.Transactions.Commands;

public static class ModifyTransaction
{
    public record Request
    {
        public required TransactionId Id { get; init; }

        public DateOnly Date { get; init; }

        public Money Amount { get; init; }

        public string Description { get; init; } = null!;

        public required AccountId AccountId { get; init; }

        public required CategoryId CategoryId { get; init; }

        public string[] Tags { get; init; } = null!;
    }
}
