using DrifterApps.Holefeeder.Framework.SeedWork;
using Microsoft.IdentityModel.Tokens;

namespace DrifterApps.Holefeeder.Hosting.OcelotGateway.API.Authorization.Google
{
    public class GoogleTokenValidationParameters : TokenValidationParameters
    {
        public GoogleTokenValidationParameters()
        {
        }

        private GoogleTokenValidationParameters(GoogleTokenValidationParameters other) : base(other)
        {
            other.ThrowIfNull(nameof(other));

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
