using System.Collections.Immutable;

using Holefeeder.Application.Features.Accounts;
using Holefeeder.Application.Features.Categories;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Application.Features.Transactions;

internal static class TransactionMapper
{
    public static TransactionInfoViewModel MapToDto(Transaction entity)
    {
        TransactionInfoViewModel dto = new()
        {
            Id = entity.Id,
            Amount = entity.Amount,
            Date = entity.Date,
            Description = entity.Description,
            Tags = entity.Tags.ToImmutableArray(),
            Account = AccountMapper.MapToAccountInfoViewModel(entity.Account!),
            Category = CategoryMapper.MapToCategoryInfoViewModel(entity.Category!)
        };

        return dto;
    }

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
            CashflowId = entity.CashflowId,
            CashflowDate = entity.CashflowDate
        };

        return dto;
    }
}
