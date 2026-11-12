using System.Text.Json;

using Holefeeder.Service.Insights.Core;

namespace Holefeeder.Service.Insights.Api;

internal sealed record BreakdownResponse(
    PeriodResponse Period,
    decimal Current,
    decimal Average,
    decimal Variation,
    decimal VariationPercentage,
    IReadOnlyList<CategoryBreakdownResponse> Categories,
    IReadOnlyList<TagBreakdownResponse> Tags)
{
    public static BreakdownResponse From(Breakdown breakdown) =>
        new(
            new PeriodResponse(breakdown.Period.Start, breakdown.Period.End),
            breakdown.Current,
            breakdown.Average,
            breakdown.Variation,
            breakdown.VariationPercentage,
            breakdown.Categories.Select(category => new CategoryBreakdownResponse(
                category.Id.Value,
                category.Name,
                category.Color,
                category.Budget,
                JsonNamingPolicy.CamelCase.ConvertName(category.BudgetStatus.ToString()),
                category.BudgetPercentage,
                category.Amount,
                category.Average,
                category.Tags.Select(tag => new TagAmountResponse(tag.Tag, tag.Amount)).ToList())).ToList(),
            breakdown.Tags.Select(tag => new TagBreakdownResponse(tag.Tag, tag.Amount, tag.Average)).ToList());
}

internal sealed record PeriodResponse(DateOnly Start, DateOnly End);

internal sealed record CategoryBreakdownResponse(
    Guid Id,
    string Name,
    string Color,
    decimal Budget,
    string BudgetStatus,
    int BudgetPercentage,
    decimal Amount,
    decimal Average,
    IReadOnlyList<TagAmountResponse> Tags);

internal sealed record TagBreakdownResponse(string Tag, decimal Amount, decimal Average);

internal sealed record TagAmountResponse(string Tag, decimal Amount);
