namespace Holefeeder.Web.Config;

public record AngularSettings
{
  public Uri ApiUrl { get; init; } = null!;
  public Uri RedirectUrl { get; init; } = null!;
}
