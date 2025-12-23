using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Application.Features.Accounts;

internal static class AccountMapper
{
    public static MyDataAccountDto MapToMyDataAccountDto(Account entity) =>
        new()
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

    public static AccountInfoViewModel MapToAccountInfoViewModel(Account entity) => new(entity.Id, entity.Name);

    public static AccountViewModel MapToAccountViewModel(Account entity, DateOnly toDate)
    {
        var balance = entity.CalculateBalance();
        var lastTransactionDate = entity.CalculateLastTransactionDate();
        var upcomingVariation = entity.CalculateUpcomingVariation(toDate);

        return new AccountViewModel(
            entity.Id,
            entity.Type,
            entity.Name,
            entity.OpenBalance,
            entity.OpenDate,
            entity.Transactions.Count,
            balance,
            lastTransactionDate,
            upcomingVariation,
            balance + upcomingVariation,
            entity.Description,
            entity.Favorite,
            entity.Inactive);
    }
}
