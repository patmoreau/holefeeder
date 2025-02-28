using System.Security.Claims;
using Auth0.OidcClient;
using Holefeeder.Ui.Shared.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace Holefeeder.Ui.Authentication;

internal class Auth0AuthenticationStateProvider(
    Auth0Client client,
    IOptions<Auth0Options> auth0Options,
    ITokenProvider tokenProvider)
    : AuthenticationStateProvider, IAuthNavigationManager
{
    private ClaimsPrincipal? _currentUser;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_currentUser is null)
        {
            await Initialize();
        }

        return new AuthenticationState(_currentUser!);
    }

    public async Task LoginAsync()
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
        var extraParameters = new Dictionary<string, string>();
        var audience = auth0Options.Value.Audience;

        if (!string.IsNullOrEmpty(audience))
        {
            extraParameters.Add("audience", audience);
        }

        try
        {
            var loginResult = await client.LoginAsync(extraParameters);
            if (!loginResult.IsError)
            {
                await tokenProvider.SetTokensAsync(new Tokens(
                    loginResult.IdentityToken,
                    loginResult.AccessToken,
                    loginResult.RefreshToken,
                    loginResult.AccessTokenExpiration));

                var claims = loginResult.User.Claims
                    .Select(x => (x.Type, x.Value))
                    .ToList();

                await tokenProvider.SetClaimsAsync(claims);

                authenticatedUser = loginResult.User;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        _currentUser = authenticatedUser;

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public async Task LogoutAsync()
    {
        await client.LogoutAsync();

        await tokenProvider.ClearAllAsync();

        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    private async Task Initialize()
    {
        var tokens = await tokenProvider.GetTokensAsync();
        var claims = await tokenProvider.GetClaimsAsync();

        if (tokens is not null && DateTime.Now < tokens.Expires)
        {
            _currentUser =
                new ClaimsPrincipal(new ClaimsIdentity(claims.Select(x => new Claim(x.Name, x.Value)), "none"));
        }
        else
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        }
    }
}