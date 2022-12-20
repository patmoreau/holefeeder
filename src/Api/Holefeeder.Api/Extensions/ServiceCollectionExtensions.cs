using System.Diagnostics.CodeAnalysis;

using Holefeeder.Api.Authorization;
using Holefeeder.Api.Swagger;
using Holefeeder.Infrastructure.SeedWork;

using MicroElements.Swashbuckle.FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Holefeeder.Api.Extensions;

[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(
                options => options.TokenValidationParameters =
                    new TokenValidationParameters {ValidateIssuer = true},
                options => configuration.Bind("AzureAdB2C", options));

        services.AddAuthorization(o =>
        {
            o.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(ClaimConstants.Scope, Policies.HOLEFEEDER_USER)
                .Build();
        });

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services, IHostEnvironment environment)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SwaggerDoc("v2",
                    new OpenApiInfo {Title = environment.ApplicationName, Version = "v2"});
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
                                    "https://holefeeder.b2clogin.com/holefeeder.onmicrosoft.com/b2c_1a_signup_signin_drifterapps/oauth2/v2.0/authorize"),
                            TokenUrl =
                                new Uri(
                                    "https://holefeeder.b2clogin.com/holefeeder.onmicrosoft.com/b2c_1a_signup_signin_drifterapps/oauth2/v2.0/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                ["openid"] = "Sign In Permissions",
                                ["profile"] = "Read profile Permission",
                                ["https://holefeeder.onmicrosoft.com/holefeeder.api/holefeeder.user"] =
                                    "API permission"
                            }
                        }
                    },
                    Description = "Azure AD Authorization Code Flow authorization",
                    In = ParameterLocation.Header,
                    Name = "Authorization"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2"}
                        },
                        new List<string>()
                    }
                });
                options.OperationFilter<QueryRequestOperationFilter>();
            })
            .AddFluentValidationRulesToSwagger();

        return services;
    }

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddCheck("api", () => HealthCheckResult.Healthy(), tags: new[] {"holefeeder", "api", "service"})
            .AddMySql(configuration.GetConnectionString(BudgetingConnectionStringBuilder.BUDGETING_CONNECTION_STRING)!,
                "budgeting-db-check", tags: new[] {"holefeeder", "api", "mysql"});

        return services;
    }
}
