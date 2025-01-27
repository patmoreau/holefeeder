namespace Holefeeder.Application.Authorization;

public class Auth0Options
{
    public string Domain { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Issuer => $"https://{Domain}/";
}
