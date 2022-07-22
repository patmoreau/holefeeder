using System.Collections.Immutable;

using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.Infrastructure.Mapping;

internal class CashflowMapper
{
    private readonly AccountMapper _accountMapper;
    private readonly CategoryMapper _categoryMapper;
    private readonly TagsMapper _tagsMapper;

    public CashflowMapper(TagsMapper tagsMapper, AccountMapper accountMapper, CategoryMapper categoryMapper)
    {
        _tagsMapper = tagsMapper;
        _accountMapper = accountMapper;
        _categoryMapper = categoryMapper;
    }

    public Cashflow? MapToModelOrNull(CashflowEntity? entity)
    {
        if (entity is null)
        {
            return null;
        }

        var model = new Cashflow()
        {
            Id=entity.Id,
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

        return model.SetTags(_tagsMapper.Map(entity.Tags));
    }

    public CashflowInfoViewModel? MapToDtoOrNull(CashflowEntity? entity)
    {
        return entity is null ? null : MapToDto(entity);
    }

    public CashflowInfoViewModel MapToDto(CashflowEntity entity)
    {
        var dto = new CashflowInfoViewModel
        {
            Id = entity.Id,
            Amount = entity.Amount,
            Description = entity.Description,
            EffectiveDate = entity.EffectiveDate,
            Frequency = entity.Frequency,
            IntervalType = entity.IntervalType,
            Recurrence = entity.Recurrence,
            Inactive = entity.Inactive,
            Tags = _tagsMapper.Map(entity.Tags).ToImmutableArray(),
            Account = _accountMapper.MapToAccountInfoViewModel(entity.Account),
            Category = _categoryMapper.MapToCategoryInfoViewModel(entity.Category)
        };

        return dto;
    }

    public IEnumerable<CashflowInfoViewModel> MapToDto(IEnumerable<CashflowEntity> entities)
    {
        return entities.Select(MapToDto);
    }

    public CashflowEntity MapToEntity(Cashflow model)
    {
        var entity = new CashflowEntity
        {
            Id = model.Id,
            AccountId = model.AccountId,
            Amount = model.Amount,
            CategoryId = model.CategoryId,
            Description = model.Description,
            EffectiveDate = model.EffectiveDate,
            Frequency = model.Frequency,
            Inactive = model.Inactive,
            IntervalType = model.IntervalType,
            Recurrence = model.Recurrence,
            Tags = _tagsMapper.Map(model.Tags),
            UserId = model.UserId
        };

        return entity;
    }

    public MyDataCashflowDto MapToMyDataCashflowDto(CashflowEntity entity)
    {
        var dto = new MyDataCashflowDto
        {
            Id = entity.Id,
            Amount = entity.Amount,
            Description = entity.Description,
            EffectiveDate = entity.EffectiveDate,
            Frequency = entity.Frequency,
            IntervalType = entity.IntervalType,
            Recurrence = entity.Recurrence,
            Tags = _tagsMapper.Map(entity.Tags).ToArray(),
            AccountId = entity.AccountId,
            CategoryId = entity.CategoryId,
            Inactive = entity.Inactive
        };

        return dto;
    }
}
