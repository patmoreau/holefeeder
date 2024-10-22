using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Accounts;

public sealed partial record Account
{
    public static Result<Account> Create(AccountType type, string name, Money openBalance, DateOnly openDate,
        string description, UserId userId)
    {
        var result = ResultAggregate.Create()
            .Ensure(NameValidation(name))
            .Ensure(OpenDateValidation(openDate))
            .Ensure(UserIdValidation(userId));

        return result.Switch(
            () => Result<Account>.Success(new Account(AccountId.New, type, name, openDate, userId)
            {
                OpenBalance = openBalance,
                Description = description
            }),
            Result<Account>.Failure);
    }

    public static Result<Account> Import(AccountId id, AccountType type, string name, Money openBalance,
        DateOnly openDate, string description, bool favorite, bool inactive, UserId userId)
    {
        var result = ResultAggregate.Create()
            .Ensure(IdValidation(id))
            .Ensure(NameValidation(name))
            .Ensure(OpenDateValidation(openDate))
            .Ensure(UserIdValidation(userId));

        return result.Switch(
            () => Result<Account>.Success(new Account(id, type, name, openDate, userId)
            {
                OpenBalance = openBalance,
                Description = description,
                Favorite = favorite,
                Inactive = inactive
            }),
            Result<Account>.Failure);
    }
}
