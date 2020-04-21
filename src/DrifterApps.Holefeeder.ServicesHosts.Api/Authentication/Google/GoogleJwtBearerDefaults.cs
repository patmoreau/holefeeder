using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DrifterApps.Holefeeder.ServicesHosts.Api.Authentication.Google
{
    /// <summary>
    /// Default values used by Google bearer authentication.
    /// </summary>
    public static class GoogleJwtBearerDefaults
    {
        /// <summary>
        /// The default authority for Google authentication.
        /// </summary>
        public const string AUTHORITY = "https://accounts.google.com";

        /// <summary>
        /// The default authentication scheme for Google authentication.
        /// </summary>
        public const string AUTHENTICATION_SCHEME = "Google." + JwtBearerDefaults.AuthenticationScheme;
    }
}
