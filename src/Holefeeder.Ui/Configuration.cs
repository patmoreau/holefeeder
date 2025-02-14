namespace Holefeeder.Ui;

internal static class Configuration
{
#if DEBUG
    public static class Auth0
    {
        public const string Authority = BaseAuth0.Authority;
        public const string Domain = BaseAuth0.Domain;
        public const string Audience = BaseAuth0.Audience;
        public const string ClientId = BaseAuth0.ClientId;
    }

    public static class DownstreamApi
    {
        public const string BaseUrl = "https://holefeeder.localtest.me/gateway";
        public const string Scopes = BaseDownstreamApi.Scopes;
    }

    public const string Environment = "Development";
#else
    public static class Auth0
    {
        public const string Authority = BaseAuth0.Authority;
        public const string Domain = BaseAuth0.Domain;
        public const string Audience = BaseAuth0.Audience;
        public const string ClientId = BaseAuth0.ClientId;
    }

    public static class DownstreamApi
    {
        public static string BaseUrl = "https://holefeeder.drifterapps.app/gateway";
        public const string Scopes = BaseDownstreamApi.Scopes;
    }

    public const string Environment = "Production";
#endif

    private class BaseAuth0
    {
        public const string Authority = "https://dev-vx1jio3owhaqdmqa.ca.auth0.com/";
        public const string Domain = "dev-vx1jio3owhaqdmqa.ca.auth0.com";
        public const string Audience = "https://holefeeder-api.drifterapps.app";
        public const string ClientId = "0IGRVkDUEJI8grrfMPHiVdOXLNjgVMdO";
    }

    private static class BaseDownstreamApi
    {
        public const string Scopes = "openid profile email offline_access read:user write:user";
    }
}
