using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Application.Features.Transactions.Queries;

public static class GetTransaction
{
    public record Request(TransactionId Id);
}
