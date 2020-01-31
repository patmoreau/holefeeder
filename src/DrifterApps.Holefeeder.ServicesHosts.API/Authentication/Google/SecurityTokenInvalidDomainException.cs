using Microsoft.IdentityModel.Tokens;

namespace Holefeeder.Services.API.Authentication.Google
{
    public class SecurityTokenInvalidDomainException : SecurityTokenValidationException
    {
        public SecurityTokenInvalidDomainException(string message) : base(message)
        {
        }

        public string InvalidDomain { get; set; }
    }
}
