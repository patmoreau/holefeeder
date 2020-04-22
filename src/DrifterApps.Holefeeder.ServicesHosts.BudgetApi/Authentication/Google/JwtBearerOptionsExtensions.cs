using DrifterApps.Holefeeder.Common.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DrifterApps.Holefeeder.ServicesHosts.BudgetApi.Authentication.Google
{
    public static class JwtBearerOptionsExtensions
    {
        public static JwtBearerOptions UseGoogle(this JwtBearerOptions options, string clientId) => options.UseGoogle(clientId, null);

        private static JwtBearerOptions UseGoogle(this JwtBearerOptions options, string clientId, string hostedDomain)
        {
            clientId = clientId.ThrowIfNullOrEmpty(nameof(clientId));
            options = options.ThrowIfNull(nameof(options));

            options.Audience = clientId;
            options.Authority = GoogleJwtBearerDefaults.AUTHORITY;

            options.SecurityTokenValidators.Clear();
            options.SecurityTokenValidators.Add(new GoogleJwtSecurityTokenHandler());

            options.TokenValidationParameters = new GoogleTokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                ValidateAudience = true,
                ValidAudience = clientId,

                ValidateIssuer = true,
                ValidIssuers = new[] { GoogleJwtBearerDefaults.AUTHORITY, "" },

                ValidateLifetime = true,

                ValidateHostedDomain = !string.IsNullOrEmpty(hostedDomain),
                HostedDomain = hostedDomain,

                NameClaimType = GoogleClaimTypes.Name,
                AuthenticationType = GoogleJwtBearerDefaults.AUTHENTICATION_SCHEME
            };

            return options;
        }
    }
}
