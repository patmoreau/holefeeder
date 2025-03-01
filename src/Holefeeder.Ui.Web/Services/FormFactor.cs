using Holefeeder.Ui.Shared.Services;

namespace Holefeeder.Ui.Web.Services;

public class FormFactor : IFormFactor
{
    public string GetFormFactor() => "Web";

    public string GetPlatform() => Environment.OSVersion.ToString();
}
