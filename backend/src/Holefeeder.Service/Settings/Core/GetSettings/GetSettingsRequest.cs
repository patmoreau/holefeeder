using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Settings.Core.GetSettings;

internal sealed record GetSettingsRequest(User User, DateOnly Today);
