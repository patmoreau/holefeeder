using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Settings.Core.SaveSettings;

internal sealed class SaveSettingsUseCase(ISettings settings)
{
    public Task<Result<Nothing>> HandleAsync(SaveSettingsRequest request, CancellationToken cancellationToken) =>
        settings.SaveAsync(request.User, request.Settings, cancellationToken);
}
