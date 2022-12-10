using System.Collections.Immutable;

using Holefeeder.Application.Features.Accounts;
using Holefeeder.Application.Features.Categories;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Application.Features.Transactions;

internal static class TransactionMapper
{
    public static Transaction? MapToModelOrNull(Transaction? entity)
    {
        if (entity is null)
        {
            return null;
        }

        var model = Transaction.Create(entity.Id, entity.Date, entity.Amount, entity.Description, entity.AccountId,
            entity.CategoryId, entity.UserId);

        if (entity.CashflowId is not null)
        {
            model = model.ApplyCashflow(entity.CashflowId.GetValueOrDefault(), entity.CashflowDate.GetValueOrDefault());
        }

        return model.SetTags(TagsMapper.Map(entity.Tags));
    }

    public static TransactionInfoViewModel MapToDto(Transaction entity)
    {
        var dto = new TransactionInfoViewModel
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

    public static IEnumerable<TransactionInfoViewModel> MapToDto(IEnumerable<Transaction> entities)
    {
        return entities.Select(MapToDto);
    }

    public static MyDataTransactionDto MapToMyDataTransactionDto(Transaction entity)
    {
        var dto = new MyDataTransactionDto
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
