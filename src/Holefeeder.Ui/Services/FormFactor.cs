using Holefeeder.Ui.Shared.Services;

namespace Holefeeder.Ui.Services;

public class FormFactor : IFormFactor
{
    public string GetFormFactor() => DeviceInfo.Idiom.ToString();

    public string GetPlatform() => DeviceInfo.Platform + " - " + DeviceInfo.VersionString;
}
