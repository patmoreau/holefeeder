using DrifterApps.Holefeeder.Common.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace Holefeeder.Services.API.Authentication.Google
{
    public class GoogleTokenValidationParameters : TokenValidationParameters
    {
        public GoogleTokenValidationParameters()
        {
        }

        protected GoogleTokenValidationParameters(GoogleTokenValidationParameters other) : base(other)
        {
            other = other.ThrowIfNull(nameof(other));

            HostedDomain = other.HostedDomain;
            ValidateHostedDomain = other.ValidateHostedDomain;
        }

        public string HostedDomain { get; set; }

        public bool ValidateHostedDomain { get; set; }

        public override TokenValidationParameters Clone()
        {
            return new GoogleTokenValidationParameters(this);
        }
    }
}
