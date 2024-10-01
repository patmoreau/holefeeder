using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Accounts;

public sealed partial record Account
{
    public static Result<Account> Create(AccountType type, string name, Money openBalance, DateOnly openDate,
        string description, UserId userId)
    {
        var result = Result.Validate(NameValidation(name), OpenDateValidation(openDate), UserIdValidation(userId));
        if (result.IsFailure)
        {
            return Result<Account>.Failure(result.Error);
        }

        return Result<Account>.Success(new Account(AccountId.New, type, name, openDate, userId)
        {
            OpenBalance = openBalance,
            Description = description
        });
    }

    public static Result<Account> Import(AccountId id, AccountType type, string name, Money openBalance, DateOnly openDate,
        string description, bool favorite, bool inactive, UserId userId)
    {
        var result = Result.Validate(IdValidation(id), NameValidation(name), OpenDateValidation(openDate), UserIdValidation(userId));
        if (result.IsFailure)
        {
            return Result<Account>.Failure(result.Error);
        }

        return Result<Account>.Success(new Account(id, type, name, openDate, userId)
        {
            OpenBalance = openBalance,
            Description = description,
            Favorite = favorite,
            Inactive = inactive
        });
    }
}
