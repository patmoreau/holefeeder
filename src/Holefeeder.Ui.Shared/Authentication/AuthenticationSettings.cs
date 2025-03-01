namespace Holefeeder.Ui.Shared.Authentication;

public class AuthenticationSettings
{
    public bool UseIdTokenForHttpAuthentication { get; set; }

    public int RefreshExpiryClockSkewInMinutes { get; set; } = 5;
}
