using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.API.Authorization;
using DrifterApps.Holefeeder.Budgeting.API.Controllers;
using DrifterApps.Holefeeder.Budgeting.API.Middlewares;
using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Infrastructure;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Application.BackgroundRequest;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v2", new() { Title = builder.Environment.ApplicationName, Version = "v2" });
        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows()
            {
                AuthorizationCode = new OpenApiOAuthFlow()
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
                        ["https://holefeeder.onmicrosoft.com/holefeeder.api/holefeeder.user"] = "API permission"
                    }
                }
            },
            Description = "Azure AD Authorization Code Flow authorization",
            In = ParameterLocation.Header,
            Name = "Authorization"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" },
                },
                new List<string>()
            }
        });
    })
    .AddApplication()
    .AddLogging()
    .AddHostedService<LongRunningService>()
    .AddSingleton<BackgroundWorkerQueue>()
    .AddTransient<Func<IRequestUser>>(provider =>
    {
        var httpContext = provider.GetService<IHttpContextAccessor>();
        return () => new RequestUserContext(httpContext?.HttpContext?.User.GetUniqueId() ?? Guid.Empty);
    })
    .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
    .AddInfrastructure(builder.Configuration);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(
        options => options.TokenValidationParameters =
            new TokenValidationParameters { ValidateIssuer = false },
        options => builder.Configuration.Bind("AzureAdB2C", options));

// Registers required services for health checks
builder.Services
    .AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddMySql(builder.Configuration["HolefeederDatabaseSettings:ConnectionString"],
        "BudgetingDB-check",
        tags: new[] { "budgeting-db" });

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error")
        .UseHsts();
}

app.AddAccountsRoutes()
    .AddCashflowsRoutes()
    .AddCategoriesRoutes()
    .AddEnumerationsRoutes()
    .AddImportsRoutes()
    .AddTransactionsRoutes()
    .UseCustomErrors(builder.Environment)
    .UseSwagger()
    .UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v2/swagger.json", $"{builder.Environment.ApplicationName} v2");
        c.OAuthClientId("9814ecda-b8db-4775-a361-714af29fe486");
        c.OAuthAppName($"{builder.Environment.ApplicationName}");
        c.OAuthScopeSeparator(" ");
        c.OAuthUsePkce();
    })
    .UseAuthentication()
    .UseAuthorization()
    .MigrateDb();

app.Run();

namespace DrifterApps.Holefeeder.Budgeting.API
{
    public partial class Program { }
}
