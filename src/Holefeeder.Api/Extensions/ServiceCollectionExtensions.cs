using System.Diagnostics.CodeAnalysis;

using Holefeeder.Api.Swagger;
using Holefeeder.Application.Authorization;
using Holefeeder.Infrastructure.SeedWork;

using MicroElements.Swashbuckle.FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

namespace Holefeeder.Api.Extensions;

[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    private const string HolefeederScope = "https://holefeeder.onmicrosoft.com/api/holefeeder.user";

    private static readonly string[] ServiceTags = ["holefeeder", "api", "service"];
    private static readonly string[] DatabaseTags = ["holefeeder", "api", "mariadb"];
    private static readonly string[] HangfireTags = ["holefeeder", "api", "hangfire"];

    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMicrosoftIdentityWebApiAuthentication(configuration, "AzureAdB2C");

        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(ClaimConstants.Scope, Policies.HolefeederUser)
                .Build());

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services, IHostEnvironment environment,
        IConfiguration configuration)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SwaggerDoc("v2",
                    new OpenApiInfo { Title = environment.ApplicationName, Version = "v2" });
                options.CustomSchemaIds(type => type.ToString());
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl =
                                new Uri(
                                    $"{configuration["AzureAdB2C:Instance"]}/{configuration["AzureAdB2C:Domain"]}/oauth2/v2.0/authorize?p={configuration["AzureAdB2C:SignUpSignInPolicyId"]}"),
                            TokenUrl = new Uri(
                                $"{configuration["AzureAdB2C:Instance"]}/{configuration["AzureAdB2C:Domain"]}/oauth2/v2.0/token?p={configuration["AzureAdB2C:SignUpSignInPolicyId"]}"),
                            Scopes = new Dictionary<string, string>
                            {
                                ["openid"] = "Sign In Permissions",
                                ["profile"] = "Read profile Permission",
                                [HolefeederScope] = "API permission"
                            }
                        }
                    }
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2"}
                        },
                        [HolefeederScope]
                    }
                });
                options.OperationFilter<QueryRequestOperationFilter>();
                options.DocumentFilter<HideInDocsFilter>();
            })
            .AddFluentValidationRulesToSwagger();

        return services;
    }

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddCheck("api", () => HealthCheckResult.Healthy(), ServiceTags)
            .AddMySql(configuration.GetConnectionString(BudgetingConnectionStringBuilder.BudgetingConnectionString)!,
                name: "holefeeder-db", tags: DatabaseTags)
            .AddHangfire(options => options.MinimumAvailableServers = 1, name: "hangfire", tags: HangfireTags);
        services
            .AddHealthChecksUI(setup => { setup.AddHealthCheckEndpoint("hc-api", "/healthz"); })
            .AddInMemoryStorage();

        return services;
    }

    public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetValue<string[]>("AllowedOrigins") ?? [];
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
}
