using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetDashboardSummary;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.Dashboard.TestKit;

internal sealed class DashboardInMemory : IDashboard
{
    private readonly Dictionary<Id, PeriodSettings> _settings = [];
    private readonly List<(Id UserId, DailyTotal Total)> _totals = [];
    private ResultError? _failure;

    public Task<Result<PeriodSettings>> GetPeriodSettingsAsync(User user, DateOnly today, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<PeriodSettings>>(_failure);
        }

        return Task.FromResult<Result<PeriodSettings>>(
            _settings.TryGetValue(user.Id, out var settings) ? settings : PeriodSettings.Default(today));
    }

    public Task<Result<IReadOnlyList<DailyTotal>>> GetDailyTotalsAsync(User user, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<IReadOnlyList<DailyTotal>>>(_failure);
        }

        IReadOnlyList<DailyTotal> totals = [.. _totals.Where(entry => entry.UserId == user.Id).Select(entry => entry.Total)];
        return Task.FromResult<Result<IReadOnlyList<DailyTotal>>>(totals.ToList());
    }

    public void Add(User user, PeriodSettings settings) => _settings[user.Id] = settings;

    public void Add(User user, DailyTotal total) => _totals.Add((user.Id, total));

    public void FailWith(ResultError error) => _failure = error;
}
