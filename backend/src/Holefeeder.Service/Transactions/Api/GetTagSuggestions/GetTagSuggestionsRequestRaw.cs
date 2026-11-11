using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core.GetTagSuggestions;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Transactions.Api.GetTagSuggestions;

internal sealed record GetTagSuggestionsRequestRaw([FromQuery(Name = "categoryId")] string? CategoryIdRaw)
{
    public static Result<GetTagSuggestionsRequest> ToGetTagSuggestionsRequest(GetTagSuggestionsRequestRaw raw, User user)
    {
        if (string.IsNullOrEmpty(raw.CategoryIdRaw))
        {
            return new GetTagSuggestionsRequest(user, null);
        }

        var categoryId = Id.Create(raw.CategoryIdRaw);

        var validation = ResultAggregate.Create();
        if (categoryId.IsFailure)
        {
            validation.AddResult(categoryId.Error);
        }

        return validation.OnSuccess<GetTagSuggestionsRequest>(() => new GetTagSuggestionsRequest(user, categoryId.Value));
    }
}
