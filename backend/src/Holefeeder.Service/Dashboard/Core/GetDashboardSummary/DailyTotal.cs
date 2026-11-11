namespace Holefeeder.Service.Dashboard.Core.GetDashboardSummary;

internal sealed record DailyTotal(CategoryType CategoryType, DateOnly Date, decimal Total);
