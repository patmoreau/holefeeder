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
    RangeResponse Range,
    IReadOnlyList<RangeOptionResponse> Ranges)
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
                .Select(flow => new PeriodFlowResponse(new PeriodResponse(flow.Period.Start, flow.Period.End), flow.Income, flow.Spending, flow.Net))
                .ToList(),
            overview.TotalIncome,
            overview.TotalSpending,
            overview.TotalNet,
            RangeOf(overview.Range),
            overview.Ranges.Select(option => new RangeOptionResponse(option.Periods, RangeOf(option.Range))).ToList());

    private static RangeResponse RangeOf(RangeSpan range) =>
        new(JsonNamingPolicy.CamelCase.ConvertName(range.Unit.ToString()), range.Amount);
}

internal sealed record PeriodFlowResponse(PeriodResponse Period, decimal Income, decimal Spending, decimal Net);

internal sealed record RangeResponse(string Unit, int Amount);

internal sealed record RangeOptionResponse(int Periods, RangeResponse Range);
