using System;
using DrifterApps.Holefeeder.Framework.SeedWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DrifterApps.Holefeeder.Hosting.OcelotGateway.API.Authorization.Google
{
    public static class JwtBearerOptionsExtensions
    {
        public static JwtBearerOptions UseGoogle(this JwtBearerOptions options, string clientId, string authority)
        {
            clientId.ThrowIfNullOrEmpty(nameof(clientId));
            authority.ThrowIfNullOrEmpty(nameof(authority));
            return options.UseGoogle(clientId, null, authority);
        }

        private static JwtBearerOptions UseGoogle(this JwtBearerOptions options, string clientId, string hostedDomain, string authority)
        {
            options.ThrowIfNull(nameof(options));
            clientId.ThrowIfNullOrEmpty(nameof(clientId));
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
                ValidIssuer = authority.Replace("https://", "", StringComparison.InvariantCulture),
                ValidIssuers = new[] { authority.Replace("https://", "", StringComparison.InvariantCulture), "" },

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
