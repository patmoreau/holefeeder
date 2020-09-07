using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DrifterApps.Holefeeder.API.Authorization.Google
{
    public static class JwtBearerOptionsExtensions
    {
        public static JwtBearerOptions UseGoogle(this JwtBearerOptions options, string clientId, string authority) => options.UseGoogle(clientId, null, authority);

        private static JwtBearerOptions UseGoogle(this JwtBearerOptions options, string clientId, string hostedDomain, string authority)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (string.IsNullOrWhiteSpace(authority))
            {
                throw new ArgumentNullException(nameof(authority));
            }

            _ = options ?? throw new ArgumentNullException(nameof(options));

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
