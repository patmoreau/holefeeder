using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core.FavoriteAccount;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Api.FavoriteAccount;

internal sealed record FavoriteAccountRequestRaw([FromBody] FavoriteAccountBody Body)
{
    public static Result<FavoriteAccountRequest> ToFavoriteAccountRequest(FavoriteAccountRequestRaw raw, User user)
    {
        var id = Id.Create(raw.Body.Id);
        var isFavorite = ParseIsFavorite(raw.Body.IsFavorite);

        var validation = ResultAggregate.Create();
        if (id.IsFailure)
        {
            validation.AddResult(id.Error);
        }
        if (isFavorite.IsFailure)
        {
            validation.AddResult(isFavorite.Error);
        }

        return validation.OnSuccess<FavoriteAccountRequest>(() => new FavoriteAccountRequest(user, id.Value, isFavorite.Value));
    }

    private static Result<bool> ParseIsFavorite(string? value) =>
        value is null ? false : bool.TryParse(value, out var isFavorite) ? isFavorite : FavoriteAccountRequestErrors.IsFavoriteInvalid;
}

internal static class FavoriteAccountRequestErrors
{
    public const string CodeIsFavoriteInvalid = "is-favorite-invalid";

    public static ResultError IsFavoriteInvalid => new(CodeIsFavoriteInvalid, "Is favorite must be true or false.");
}
