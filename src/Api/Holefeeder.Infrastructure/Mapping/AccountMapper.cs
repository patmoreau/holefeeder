using System.Collections.Immutable;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.Infrastructure.Mapping;

internal class AccountMapper
{
    public MyDataAccountDto MapToMyDataAccountDto(AccountEntity entity)
    {
        return new()
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
    }

    public AccountInfoViewModel MapToAccountInfoViewModel(AccountEntity entity)
    {
        return new(entity.Id, entity.Name);
    }

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
