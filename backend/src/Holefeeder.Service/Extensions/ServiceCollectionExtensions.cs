using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Holefeeder.Service.Extensions;

[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    private const string Auth0Scheme = "Auth0";
    private const string ReadUserPolicy = "ReadUser";
    private const string WriteUserPolicy = "WriteUser";
    private const string ReadUserScope = "read:user";
    private const string WriteUserScope = "write:user";

    private static readonly string[] ServiceTags = ["holefeeder", "api", "service"];

    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(Auth0Scheme, options =>
            {
                configuration.Bind($"Authorization:{Auth0Scheme}", options);

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = OnTokenValidated
                };
            });

        services
            .AddAuthorization(options =>
            {
                options.AddPolicy(ReadUserPolicy, policy =>
                    policy
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(Auth0Scheme)
                        .RequireClaim("scope", ReadUserScope));
                options.AddPolicy(WriteUserPolicy, policy =>
                    policy
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(Auth0Scheme)
                        .RequireClaim("scope", WriteUserScope));
            });

        return services;
    }

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddCheck("api", () => HealthCheckResult.Healthy(), ServiceTags);

        return services;
    }

    public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

    private static Task OnTokenValidated(TokenValidatedContext context)
    {
        if (context.Principal?.Identity is not ClaimsIdentity claimsIdentity)
        {
            return Task.CompletedTask;
        }

        var scopeClaim = claimsIdentity.FindFirst("scope");
        if (scopeClaim == null)
        {
            return Task.CompletedTask;
        }

        var scopes = scopeClaim.Value.Split(' ');
        foreach (var scope in scopes)
        {
            claimsIdentity.AddClaim(new Claim("scope", scope));
        }

        claimsIdentity.RemoveClaim(scopeClaim);

        return Task.CompletedTask;
    }
}
