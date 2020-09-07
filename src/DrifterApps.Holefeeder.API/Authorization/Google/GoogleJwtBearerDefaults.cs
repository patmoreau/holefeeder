using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DrifterApps.Holefeeder.API.Authorization.Google
{
    public static class GoogleJwtBearerDefaults
    {
        public const string AUTHENTICATION_SCHEME = "Google." + JwtBearerDefaults.AuthenticationScheme;
    }
}
