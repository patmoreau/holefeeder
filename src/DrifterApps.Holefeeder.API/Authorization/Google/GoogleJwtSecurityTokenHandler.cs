using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DrifterApps.Holefeeder.API.Authorization.Google
{
    public class GoogleJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        public GoogleJwtSecurityTokenHandler()
        {
            InboundClaimTypeMap.Clear();
        }

        /// <inheritdoc />
        /// <exception cref="SecurityTokenValidationException">token 'hd' claim did not match HostedDomain.</exception>
        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            // The base class should already take care of validating signature, issuer,
            // audience and expiry. We just need to handle the hosted domain validation below.
            var principal = base.ValidateToken(token, validationParameters, out validatedToken);

            if (!(validationParameters is GoogleTokenValidationParameters googleParameters))
            {
                return principal;
            }

            var domain = googleParameters.HostedDomain;

            // No domain specified. Skip validation.
            if (string.IsNullOrEmpty(domain))
            {
                return principal;
            }

            if (googleParameters.ValidateHostedDomain)
            {
                ValidateHostedDomain(domain, principal);
            }

            return principal;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        private static void ValidateHostedDomain(string expectedDomain, ClaimsPrincipal principal)
        {
            var actualDomain = principal.FindFirst(GoogleClaimTypes.Domain)?.Value;

            if (string.IsNullOrEmpty(actualDomain))
            {
                throw LogHelper.LogExceptionMessage(new SecurityTokenValidationException(LogMessages.IDX10250));
            }

            if (!actualDomain.Equals(expectedDomain, StringComparison.OrdinalIgnoreCase))
            {
                var message = string.Format(CultureInfo.InvariantCulture, LogMessages.IDX10251, actualDomain, expectedDomain);

                throw LogHelper.LogExceptionMessage(new SecurityTokenValidationException(message));
            }

            LogHelper.LogInformation(LogMessages.IDX10252, actualDomain);
        }
    }
}
