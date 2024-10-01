using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Application.Features.Accounts;

internal static class AccountMapper
{
    public static MyDataAccountDto MapToMyDataAccountDto(Account entity) =>
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

    public static AccountInfoViewModel MapToAccountInfoViewModel(Account entity) =>
        new AccountInfoViewModel(entity.Id, entity.Name);

    public static AccountViewModel MapToAccountViewModel(Account entity) =>
        new AccountViewModel(
            entity.Id,
            entity.Type,
            entity.Name,
            entity.OpenBalance,
            entity.OpenDate,
            entity.Transactions.Count,
            entity.OpenBalance +
            entity.Transactions.Sum(x => x.Amount * x.Category?.Type.Multiplier * entity.Type.Multiplier ?? 0),
            entity.Transactions.Count > 0 ? entity.Transactions.Max(x => x.Date) : entity.OpenDate,
            entity.Description,
            entity.Favorite,
            entity.Inactive);
}
