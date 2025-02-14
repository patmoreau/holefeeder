namespace Holefeeder.Ui.Shared.Authentication;

public class Auth0Options
{
    public const string Section = "Auth0";

    public required string Domain { get; init; }
    public required string ClientId { get; init; }
    public required string Audience { get; init; }

    public required Uri Authority { get; init; }
}
