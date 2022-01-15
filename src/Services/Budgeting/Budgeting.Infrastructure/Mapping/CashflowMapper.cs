using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.MyData.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Mapping;

public class CashflowMapper
{
    private readonly TagsMapper _tagsMapper;
    private readonly AccountMapper _accountMapper;
    private readonly CategoryMapper _categoryMapper;

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

        var model = new Cashflow(entity.Id, entity.EffectiveDate, entity.Amount, entity.UserId)
        {
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

        return model.AddTags(_tagsMapper.Map(entity.Tags));
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
            Tags = _tagsMapper.Map(entity.Tags).ToImmutableArray(),
            Account = _accountMapper.MapToAccountInfoViewModel(entity.Account),
            Category = _categoryMapper.MapToCategoryInfoViewModel(entity.Category)
        };

        return dto;
    }

    public IEnumerable<CashflowInfoViewModel> MapToDto(IEnumerable<CashflowEntity> entities) =>
        entities.Select(MapToDto);

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
            UserId = model.UserId,
        };

        return entity;
    }
    
    public MyDataCashflowDto MapToExportDto(CashflowEntity entity)
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
            Tags = _tagsMapper.Map(entity.Tags).ToImmutableArray(),
            AccountId = entity.AccountId,
            CategoryId = entity.CategoryId
        };

        return dto;
    }
}
