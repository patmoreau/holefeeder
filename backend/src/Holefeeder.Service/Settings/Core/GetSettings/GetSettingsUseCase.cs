using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Settings.Core.GetSettings;

internal sealed class GetSettingsUseCase(ISettings settings)
{
    public Task<Result<PeriodSettings>> HandleAsync(GetSettingsRequest request, CancellationToken cancellationToken) =>
        settings.GetAsync(request.User, request.Today, cancellationToken);
}
