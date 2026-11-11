using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Application.Features.Transactions.Queries;

public static class GetTransaction
{
    public record Request(TransactionId Id);

    internal class Validator : AbstractValidator<Guid>
    {
        public Validator() => RuleFor(x => TransactionId.Create(x)).NotEqual(TransactionId.Empty);
    }
}
