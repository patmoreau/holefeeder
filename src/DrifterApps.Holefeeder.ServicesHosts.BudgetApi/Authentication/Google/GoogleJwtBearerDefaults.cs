using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DrifterApps.Holefeeder.ServicesHosts.BudgetApi.Authentication.Google
{
    public static class GoogleJwtBearerDefaults
    {
        public const string AUTHENTICATION_SCHEME = "Google." + JwtBearerDefaults.AuthenticationScheme;
    }
}
