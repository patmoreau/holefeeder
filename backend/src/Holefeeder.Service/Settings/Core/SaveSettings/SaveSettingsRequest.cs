using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Settings.Core.SaveSettings;

internal sealed record SaveSettingsRequest(User User, PeriodSettings Settings);
