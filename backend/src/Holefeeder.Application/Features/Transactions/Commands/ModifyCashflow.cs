using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Application.Features.Transactions.Commands;

public static class ModifyCashflow
{
    public record Request
    {
        public required CashflowId Id { get; init; }

        public required Money Amount { get; init; }

        public DateOnly EffectiveDate { get; init; }

        public string Description { get; init; } = null!;

        public string[] Tags { get; init; } = [];
    }
}
