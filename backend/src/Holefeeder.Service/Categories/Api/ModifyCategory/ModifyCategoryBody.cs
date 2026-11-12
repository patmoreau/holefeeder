using System.Text.Json.Serialization;

using Holefeeder.Service.Shared.Api;

namespace Holefeeder.Service.Categories.Api.ModifyCategory;

internal sealed record ModifyCategoryBody(
    string? Id,
    string? Name,
    string? Type,
    string? Color,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? BudgetAmount,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? Favorite);
