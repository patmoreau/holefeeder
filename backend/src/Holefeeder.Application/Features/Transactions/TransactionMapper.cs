using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Application.Features.Transactions;

internal static class TransactionMapper
{
    public static MyDataTransactionDto MapToMyDataTransactionDto(Transaction entity)
    {
        MyDataTransactionDto dto = new()
        {
            Id = entity.Id,
            Amount = entity.Amount,
            Date = entity.Date,
            Description = entity.Description,
            Tags = entity.Tags.ToArray(),
            AccountId = entity.AccountId,
            CategoryId = entity.CategoryId,
            CashflowId = entity.CashflowId?.Value,
            CashflowDate = entity.CashflowDate
        };

        return dto;
    }
}
