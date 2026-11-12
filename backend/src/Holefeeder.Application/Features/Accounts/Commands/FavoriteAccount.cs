using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Application.Features.Accounts.Commands;

public static class FavoriteAccount
{
    public record Request(AccountId Id, bool IsFavorite);
}
