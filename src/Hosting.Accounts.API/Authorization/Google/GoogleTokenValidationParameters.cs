using System;
using Microsoft.IdentityModel.Tokens;

namespace DrifterApps.Holefeeder.API.Authorization.Google
{
    public class GoogleTokenValidationParameters : TokenValidationParameters
    {
        public GoogleTokenValidationParameters()
        {
        }

        private GoogleTokenValidationParameters(GoogleTokenValidationParameters other) : base(other)
        {
            _ = other ?? throw new ArgumentNullException(nameof(other));

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
