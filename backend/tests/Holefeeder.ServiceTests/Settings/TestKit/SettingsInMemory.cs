using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Settings.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.Settings.TestKit;

internal sealed class SettingsInMemory : ISettings
{
    private readonly Dictionary<Id, PeriodSettings> _settings = [];
    private ResultError? _failure;

    public Task<Result<PeriodSettings>> GetAsync(User user, DateOnly today, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<PeriodSettings>>(_failure);
        }

        return Task.FromResult<Result<PeriodSettings>>(
            _settings.TryGetValue(user.Id, out var settings) ? settings : PeriodSettings.Default(today));
    }

    public Task<Result<Nothing>> SaveAsync(User user, PeriodSettings settings, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<Nothing>>(_failure);
        }

        _settings[user.Id] = settings;
        return Task.FromResult<Result<Nothing>>(Nothing.Value);
    }

    public void Add(User user, PeriodSettings settings) => _settings[user.Id] = settings;

    public void FailWith(ResultError error) => _failure = error;
}
