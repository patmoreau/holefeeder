using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Settings.Core;

internal interface ISettings
{
    Task<Result<PeriodSettings>> GetAsync(User user, DateOnly today, CancellationToken cancellationToken);

    Task<Result<Nothing>> SaveAsync(User user, PeriodSettings settings, CancellationToken cancellationToken);
}
