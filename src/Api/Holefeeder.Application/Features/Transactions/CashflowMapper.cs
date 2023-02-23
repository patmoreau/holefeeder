using System.Collections.Immutable;
using Holefeeder.Application.Features.Accounts;
using Holefeeder.Application.Features.Categories;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Application.Features.Transactions;

internal static class CashflowMapper
{
    public static Cashflow? MapToModelOrNull(Cashflow? entity)
    {
        if (entity is null)
        {
            return null;
        }

        Cashflow model = new Cashflow
        {
            Id = entity.Id,
            AccountId = entity.AccountId,
            Amount = entity.Amount,
            CategoryId = entity.CategoryId,
            Description = entity.Description,
            EffectiveDate = entity.EffectiveDate,
            Frequency = entity.Frequency,
            Inactive = entity.Inactive,
            IntervalType = entity.IntervalType,
            Recurrence = entity.Recurrence,
            UserId = entity.UserId
        };

        return model.SetTags(TagsMapper.Map(entity.Tags));
    }

    public static CashflowInfoViewModel? MapToDtoOrNull(Cashflow? entity) => entity is null ? null : MapToDto(entity);

    public static CashflowInfoViewModel MapToDto(Cashflow entity)
    {
        CashflowInfoViewModel dto = new CashflowInfoViewModel
        {
            Id = entity.Id,
            Amount = entity.Amount,
            Description = entity.Description,
            EffectiveDate = entity.EffectiveDate,
            Frequency = entity.Frequency,
            IntervalType = entity.IntervalType,
            Recurrence = entity.Recurrence,
            Inactive = entity.Inactive,
            Tags = entity.Tags.ToImmutableArray(),
            Account = AccountMapper.MapToAccountInfoViewModel(entity.Account!),
            Category = CategoryMapper.MapToCategoryInfoViewModel(entity.Category!)
        };

        return dto;
    }

    public static IEnumerable<CashflowInfoViewModel> MapToDto(IEnumerable<Cashflow> entities) =>
        entities.Select(MapToDto);

    public static MyDataCashflowDto MapToMyDataCashflowDto(Cashflow entity)
    {
        MyDataCashflowDto dto = new MyDataCashflowDto
        {
            Id = entity.Id,
            Amount = entity.Amount,
            Description = entity.Description,
            EffectiveDate = entity.EffectiveDate,
            Frequency = entity.Frequency,
            IntervalType = entity.IntervalType,
            Recurrence = entity.Recurrence,
            Tags = entity.Tags.ToArray(),
            AccountId = entity.AccountId,
            CategoryId = entity.CategoryId,
            Inactive = entity.Inactive
        };

        return dto;
    }
}
