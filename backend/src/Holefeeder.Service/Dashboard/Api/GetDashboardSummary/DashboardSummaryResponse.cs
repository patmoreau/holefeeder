namespace Holefeeder.Service.Dashboard.Api.GetDashboardSummary;

internal sealed record DashboardSummaryResponse(
    decimal CurrentExpenses,
    decimal ExpenseVariation,
    decimal ExpenseVariationPercentage,
    decimal NetFlow,
    decimal CurrentGains,
    decimal AverageExpenses,
    decimal ProjectedNetFlow);
