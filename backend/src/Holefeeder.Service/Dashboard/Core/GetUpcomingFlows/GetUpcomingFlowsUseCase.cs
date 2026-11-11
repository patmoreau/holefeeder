using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Dashboard.Core.GetUpcomingFlows;

internal sealed class GetUpcomingFlowsUseCase(IDashboard dashboard)
{
    public async Task<Result<IReadOnlyList<DashboardUpcomingFlow>>> HandleAsync(GetUpcomingFlowsRequest request, CancellationToken cancellationToken)
    {
        var settingsResult = await dashboard.GetPeriodSettingsAsync(request.User, request.Today, cancellationToken);
        if (settingsResult.IsFailure)
        {
            return settingsResult.Error;
        }

        var cashflowsResult = await dashboard.GetUpcomingCashflowsAsync(request.User, cancellationToken);
        if (cashflowsResult.IsFailure)
        {
            return cashflowsResult.Error;
        }

        var periodEnd = Period.Containing(request.Today, settingsResult.Value).End;

        return cashflowsResult.Value
            .SelectMany(cashflow => cashflow.Schedule.UpcomingDates(periodEnd).Select(date => ToUpcomingFlow(cashflow, date)))
            .OrderBy(flow => flow.Date)
            .ThenBy(flow => flow.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static DashboardUpcomingFlow ToUpcomingFlow(UpcomingCashflow cashflow, DateOnly date)
    {
        var title = FlowTitle.Of(cashflow.Description, cashflow.CategoryName);

        return new DashboardUpcomingFlow(
            cashflow.Id,
            date,
            title.Title,
            title.Category,
            cashflow.CategoryColor,
            cashflow.Tags,
            cashflow.Schedule.Amount,
            cashflow.Schedule.CategoryType);
    }
}
