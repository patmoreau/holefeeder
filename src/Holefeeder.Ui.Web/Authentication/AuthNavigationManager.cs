using Holefeeder.Ui.Shared.Authentication;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Holefeeder.Ui.Web.Authentication;

internal class AuthNavigationManager(NavigationManager navigation) : IAuthNavigationManager
{
    public Task LoginAsync()
    {
        navigation.NavigateToLogin("authentication/login");
        return Task.CompletedTask;
    }

    public Task LogoutAsync()
    {
        navigation.NavigateToLogout("authentication/logout");
        return Task.CompletedTask;
    }
}
