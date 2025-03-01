namespace Holefeeder.Ui.Shared.Authentication;

public interface IAuthNavigationManager
{
    Task LoginAsync();
    Task LogoutAsync();
}
