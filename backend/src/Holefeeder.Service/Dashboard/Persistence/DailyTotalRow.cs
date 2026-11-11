using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetDashboardSummary;

namespace Holefeeder.Service.Dashboard.Persistence;

internal sealed record DailyTotalRow(string CategoryType, DateOnly Date, decimal Total)
{
    public DailyTotal ToDailyTotal() => new(Enum.Parse<CategoryType>(CategoryType, ignoreCase: true), Date, Total);
}
