using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

using Holefeeder.Infrastructure.SeedWork;

using IdentityModel;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using static Holefeeder.Application.Authorization.Configuration;

namespace Holefeeder.Api.Extensions;

[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    internal const string AllowBlazorClientPolicy = nameof(AllowBlazorClientPolicy);
    private static readonly string[] ServiceTags = ["holefeeder", "api", "service"];
    private static readonly string[] DatabaseTags = ["holefeeder", "api", "postgres"];
    private static readonly string[] HangfireTags = ["holefeeder", "api", "hangfire"];

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
                        .RequireClaim(JwtClaimTypes.Scope, Scopes.ReadUser));
                options.AddPolicy(Policies.WriteUser, policy =>
                    policy
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(Schemes.Auth0)
                        .RequireClaim(JwtClaimTypes.Scope, Scopes.WriteUser));
            });

        return services;
    }

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddCheck("api", () => HealthCheckResult.Healthy(), ServiceTags)
            .AddNpgSql(provider =>
                {
                    var builder = provider.GetRequiredService<BudgetingConnectionStringBuilder>();
                    return builder.CreateBuilder().ConnectionString;
                },
                name: "holefeeder-db", tags: DatabaseTags)
            .AddHangfire(options => options.MinimumAvailableServers = 1, name: "hangfire", tags: HangfireTags);
        services
            .AddHealthChecksUI(setup => { setup.AddHealthCheckEndpoint("hc-api", "http://127.0.0.1/healthz"); })
            .AddInMemoryStorage();

        return services;
    }

    public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(options =>
        {
            options.AddPolicy(AllowBlazorClientPolicy, builder =>
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
