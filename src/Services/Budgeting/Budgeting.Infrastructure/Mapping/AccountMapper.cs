using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.MyData.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Mapping;

public class AccountMapper
{
    public MyDataAccountDto MapToExportDto(AccountEntity entity) =>
        new MyDataAccountDto
        {
            Id = entity.Id,
            Description = entity.Description,
            Favorite = entity.Favorite,
            Name = entity.Name,
            OpenBalance = entity.OpenBalance,
            OpenDate = entity.OpenDate,
            Type = entity.Type,
            Inactive = entity.Inactive
        };
    
    public AccountInfoViewModel MapToAccountInfoViewModel(AccountEntity entity) =>
        new AccountInfoViewModel(entity.Id, entity.Name);
    
    public Account? MapToModelOrNull(AccountEntity? entity, IEnumerable<Guid>? cashflows = null)
    {
        if (entity is null)
        {
            return null;
        }

        var model = new Account(entity.Id, entity.Type, entity.Name, entity.OpenDate, entity.UserId)
        {
            Favorite = entity.Favorite,
            Cashflows = cashflows?.ToImmutableArray() ?? ImmutableArray<Guid>.Empty,
            Description = entity.Description,
            Inactive = entity.Inactive,
            OpenBalance = entity.OpenBalance
        };

        return model;
    }

    public AccountEntity MapToEntity(Account model)
    {
        return new AccountEntity
        {
            Id = model.Id,
            Name = model.Name,
            Type = model.Type,
            OpenBalance = model.OpenBalance,
            OpenDate = model.OpenDate,
            Favorite = model.Favorite,
            Description = model.Description,
            Inactive = model.Inactive,
            UserId = model.UserId
        };
    }
}
