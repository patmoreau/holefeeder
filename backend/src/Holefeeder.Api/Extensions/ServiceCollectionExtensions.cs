using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using static Holefeeder.Application.Authorization.Configuration;

namespace Holefeeder.Api.Extensions;

[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    private static readonly string[] ServiceTags = ["holefeeder", "api", "service"];

    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(Schemes.Auth0, options =>
            {
                configuration.Bind($"Authorization:{Schemes.Auth0}", options);

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = OnTokenValidated
                };
            });

        services
            .AddAuthorization(options =>
            {
                options.AddPolicy(Policies.ReadUser, policy =>
                    policy
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(Schemes.Auth0)
                        .RequireClaim("scope", Scopes.ReadUser));
                options.AddPolicy(Policies.WriteUser, policy =>
                    policy
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(Schemes.Auth0)
                        .RequireClaim("scope", Scopes.WriteUser));
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
