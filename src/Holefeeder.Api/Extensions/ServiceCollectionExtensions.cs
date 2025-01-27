using System.Diagnostics.CodeAnalysis;

using Holefeeder.Api.Swagger;
using Holefeeder.Application.Authorization;
using Holefeeder.Infrastructure.SeedWork;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
        var auth0Options = new Auth0Options();
        configuration.Bind("Authorization:Auth0", auth0Options);
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MetadataAddress = $"{auth0Options.Issuer}.well-known/openid-configuration";
                options.Audience = auth0Options.Audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = auth0Options.Issuer
                };
            });

        services
            .AddAuthorization(options =>
            {
                options.AddPolicy(Configuration.Policies.ReadUser, policy =>
                    policy
                        .RequireAuthenticatedUser()
                        .Requirements
                        .Add(new HasScopeRequirement(Configuration.Scopes.ReadUser, auth0Options.Issuer)));
                options.AddPolicy(Configuration.Policies.WriteUser, policy =>
                    policy
                        .RequireAuthenticatedUser()
                        .Requirements
                        .Add(new HasScopeRequirement(Configuration.Scopes.WriteUser, auth0Options.Issuer)));
            });

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
                    new OpenApiInfo {Title = environment.ApplicationName, Version = "v2"});
                options.CustomSchemaIds(type => type.ToString());
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"https://{configuration["Auth0:Domain"]}/oauth/token"),
                            AuthorizationUrl =
                                new Uri(
                                    $"https://{configuration["Auth0:Domain"]}/authorize?audience={configuration["Auth0:Audience"]}"),
                            Scopes = new Dictionary<string, string>
                            {
                                ["openid"] = "Sign In Permissions",
                                ["profile"] = "Read profile Permission",
                                [Configuration.Policies.ReadUser] = "read:user",
                                [Configuration.Policies.WriteUser] = "write:user",
                            }
                        }
                    }
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            },
                            Scheme = "oauth2",
                            Name = "oauth2",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
                options.OperationFilter<QueryRequestOperationFilter>();
                options.DocumentFilter<HideInDocsFilter>();
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
}
