using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Application.Features.Transactions.Commands;

public static class DeleteTransaction
{
    public record Request(TransactionId Id);

    internal class Validator : AbstractValidator<Guid>
    {
        public Validator() => RuleFor(id => TransactionId.Create(id)).NotNull().NotEqual(TransactionId.Empty);
    }
}
