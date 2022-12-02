using System.Collections.Immutable;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Application.Features.Accounts;

internal static class AccountMapper
{
    public static MyDataAccountDto MapToMyDataAccountDto(Account entity)
    {
        return new MyDataAccountDto
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

    public static AccountInfoViewModel MapToAccountInfoViewModel(Account entity)
    {
        return new AccountInfoViewModel(entity.Id, entity.Name);
    }

    public static Account? MapToModelOrNull(Account? entity, IEnumerable<Guid>? cashflows = null)
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
}
