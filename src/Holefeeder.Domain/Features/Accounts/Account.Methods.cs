using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Accounts;

public sealed partial record Account
{
    public Result<Account> Close()
    {
        if (Inactive)
        {
            return Result<Account>.Failure(AccountErrors.AccountClosed);
        }

        if (Cashflows.Count > 0)
        {
            return Result<Account>.Failure(AccountErrors.ActiveCashflows);
        }

        return Result<Account>.Success(this with { Inactive = true });
    }

    public Result<Account> Modify(AccountType? type = null, string? name = null, Money? openBalance = null,
        DateOnly? openDate = null, string? description = null, bool? favorite = null, bool? inactive = null)
    {
        var newType = type ?? Type;
        var newName = name ?? Name;
        var newOpenBalance = openBalance ?? OpenBalance;
        var newOpenDate = openDate ?? OpenDate;
        var newDescription = description ?? Description;
        var newFavorite = favorite ?? Favorite;
        var newInactive = inactive ?? Inactive;

        var result = Result.Validate(NameValidation(newName), OpenDateValidation(newOpenDate));
        if (result.IsFailure)
        {
            return Result<Account>.Failure(result.Error);
        }

        return Result<Account>.Success(this with
        {
            Type = newType,
            Name = newName,
            OpenBalance = newOpenBalance,
            OpenDate = newOpenDate,
            Description = newDescription,
            Favorite = newFavorite,
            Inactive = newInactive
        });
    }
}
