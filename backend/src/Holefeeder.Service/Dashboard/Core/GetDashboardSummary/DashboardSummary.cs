namespace Holefeeder.Service.Dashboard.Core.GetDashboardSummary;

internal sealed record DashboardSummary(
    decimal CurrentExpenses,
    decimal ExpenseVariation,
    decimal ExpenseVariationPercentage,
    decimal NetFlow,
    decimal CurrentGains,
    decimal AverageExpenses,
    decimal ProjectedNetFlow);
