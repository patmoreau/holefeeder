using System.Text.Json;

using Holefeeder.Service.Insights.Core.GetOverview;

namespace Holefeeder.Service.Insights.Api.GetOverview;

internal sealed record OverviewResponse(
    PeriodResponse Period,
    decimal Income,
    decimal Spending,
    decimal Net,
    string Outcome,
    int SavedPercentage,
    IReadOnlyList<PeriodFlowResponse> Series,
    decimal TotalIncome,
    decimal TotalSpending,
    decimal TotalNet,
    RangeResponse Range)
{
    public static OverviewResponse From(Overview overview) =>
        new(
            new PeriodResponse(overview.Period.Start, overview.Period.End),
            overview.Income,
            overview.Spending,
            overview.Net,
            JsonNamingPolicy.CamelCase.ConvertName(overview.Outcome.ToString()),
            overview.SavedPercentage,
            overview.Series
                .Select(flow => new PeriodFlowResponse(new PeriodResponse(flow.Period.Start, flow.Period.End), flow.Income, flow.Spending))
                .ToList(),
            overview.TotalIncome,
            overview.TotalSpending,
            overview.TotalNet,
            new RangeResponse(JsonNamingPolicy.CamelCase.ConvertName(overview.Range.Unit.ToString()), overview.Range.Amount));
}

internal sealed record PeriodFlowResponse(PeriodResponse Period, decimal Income, decimal Spending);

internal sealed record RangeResponse(string Unit, int Amount);
