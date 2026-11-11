using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Accounts.Core;

internal sealed record Account(
    Id Id,
    AccountType Type,
    AccountName Name,
    DateOnly OpenDate,
    Money OpenBalance,
    string Description,
    bool Favorite,
    bool Inactive)
{
    public static Account Open(Id id, AccountType type, AccountName name, DateOnly openDate, Money openBalance, string description) =>
        new(id, type, name, openDate, openBalance, description, Favorite: false, Inactive: false);

    public Account Modify(AccountName name, Money? openBalance, string? description) =>
        this with { Name = name, OpenBalance = openBalance ?? OpenBalance, Description = description ?? Description };
}
