using DrifterApps.Holefeeder.Common.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DrifterApps.Holefeeder.ServicesHosts.BudgetApi.Authentication.Google
{
    public static class JwtBearerOptionsExtensions
    {
        public static JwtBearerOptions UseGoogle(this JwtBearerOptions options, string clientId, string authority) => options.UseGoogle(clientId, null, authority);

        private static JwtBearerOptions UseGoogle(this JwtBearerOptions options, string clientId, string hostedDomain, string authority)
        {
            clientId.ThrowIfNullOrEmpty(nameof(clientId));
            options.ThrowIfNull(nameof(options));
            authority.ThrowIfNullOrEmpty(nameof(authority));

            options.Audience = clientId;
            options.Authority = authority;

            options.SecurityTokenValidators.Clear();
            options.SecurityTokenValidators.Add(new GoogleJwtSecurityTokenHandler());

            options.TokenValidationParameters = new GoogleTokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                ValidateAudience = true,
                ValidAudience = clientId,

                ValidateIssuer = true,
                ValidIssuers = new[] { authority, "" },

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
