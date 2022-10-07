namespace Holefeeder.Infrastructure.Mapping;

using System.Collections.Immutable;

using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Infrastructure.Entities;

internal static class TransactionMapper
{
    public static Transaction? MapToModelOrNull(TransactionEntity? entity)
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

    public static TransactionInfoViewModel? MapToDtoOrNull(TransactionEntity? entity)
    {
        return entity is null ? null : MapToDto(entity);
    }

    public static TransactionInfoViewModel MapToDto(TransactionEntity entity)
    {
        var dto = new TransactionInfoViewModel
        {
            Id = entity.Id,
            Amount = entity.Amount,
            Date = entity.Date,
            Description = entity.Description,
            Tags = TagsMapper.Map(entity.Tags).ToImmutableArray(),
            Account = AccountMapper.MapToAccountInfoViewModel(entity.Account),
            Category = CategoryMapper.MapToCategoryInfoViewModel(entity.Category)
        };

        return dto;
    }

    public static IEnumerable<TransactionInfoViewModel> MapToDto(IEnumerable<TransactionEntity> entities)
    {
        return entities.Select(MapToDto);
    }

    public static TransactionEntity MapToEntity(Transaction model)
    {
        var entity = new TransactionEntity
        {
            Id = model.Id,
            AccountId = model.AccountId,
            Amount = model.Amount,
            CashflowDate = model.CashflowDate,
            CashflowId = model.CashflowId,
            CategoryId = model.CategoryId,
            Date = model.Date,
            Description = model.Description,
            Tags = TagsMapper.Map(model.Tags),
            UserId = model.UserId
        };

        return entity;
    }

    public static MyDataTransactionDto MapToMyDataTransactionDto(TransactionEntity entity)
    {
        var dto = new MyDataTransactionDto
        {
            Id = entity.Id,
            Amount = entity.Amount,
            Date = entity.Date,
            Description = entity.Description,
            Tags = TagsMapper.Map(entity.Tags).ToArray(),
            AccountId = entity.AccountId,
            CategoryId = entity.CategoryId,
            CashflowId = entity.CashflowId,
            CashflowDate = entity.CashflowDate
        };

        return dto;
    }
}
