using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Application.Features.Transactions.Commands;

public static class DeleteTransaction
{
    public record Request(TransactionId Id);
}
