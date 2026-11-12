using System.Text.Json;

using Carter;

using Dapper;

using DrifterApps.Seeds.Application.Converters;
using DrifterApps.Seeds.Infrastructure;

using Holefeeder.Application.Converters;
using Holefeeder.Application.Extensions;
using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.SeedWork;
using Holefeeder.Service;
using Holefeeder.Service.Accounts;
using Holefeeder.Service.Cashflows;
using Holefeeder.Service.Categories;
using Holefeeder.Service.Dashboard;
using Holefeeder.Service.ErrorHandling;
using Holefeeder.Service.Extensions;
using Holefeeder.Service.Home;
using Holefeeder.Service.Settings;
using Holefeeder.Service.StoreItems;
using Holefeeder.Service.Support;
using Holefeeder.Service.Transactions;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi;

using Npgsql;

using Scalar.AspNetCore;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddCors(builder.Configuration)
    .AddSecurity(builder.Configuration)
    .AddCarter(configurator: configurator => configurator.WithEmptyValidators())
    .AddOpenApi("v2", options =>
    {
        options.AddBearerTokenAuthentication();
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Info.Contact = new OpenApiContact
            {
                Name = "Drifter Apps Inc.",
                Email = "info@drifterapps.com"
            };
            return Task.CompletedTask;
        });
    });

if (!builder.Environment.IsEnvironment("SECURITY_TESTS"))
{
    builder.Services.AddHangfireServices();
    builder.Services.AddHealthChecks(builder.Configuration);
}

builder.Services
    .AddValidation()
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddHangfireRequestScheduler(() =>
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new StronglyTypedIdJsonConverterFactory());
        options.Converters.Add(new MoneyJsonConverterFactory());
        options.Converters.Add(new CategoryColorJsonConverterFactory());
        return options;
    });

builder.Services
    .AddSingleton(provider =>
        NpgsqlDataSource.Create(provider.GetRequiredService<BudgetingConnectionStringBuilder>().ConnectionString))
    .AddStoreItemsServices()
    .AddAccountsServices()
    .AddDashboardServices()
    .AddCashflowsServices()
    .AddTransactionsServices()
    .AddSettingsServices()
    .AddCategoriesServices()
    .AddSupportServices();

builder.Services.AddOptions<ScalarOptions>().BindConfiguration("Scalar");
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new StronglyTypedIdJsonConverterFactory());
    options.SerializerOptions.Converters.Add(new MoneyJsonConverterFactory());
    options.SerializerOptions.Converters.Add(new CategoryColorJsonConverterFactory());
});

DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = builder.Build();

// Configure for reverse proxy (Traefik, etc.)
var proxyPrefix = app.Configuration.GetValue<string>(Constants.ProxyPrefix);
if (!string.IsNullOrEmpty(proxyPrefix))
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
                           | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost
                           | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
    });
    app.UsePathBase(proxyPrefix);
}

app.UseSerilogRequestLogging();
app.UseCustomErrors(app.Environment);

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
else
{
    IdentityModelEventSource.ShowPII = true;
}

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Servers = [];
    options.Authentication = new()
    {
        PreferredSecuritySchemes = [IdentityConstants.BearerScheme]
    };
});

app
    .UseRouting()
    .UseCors()
    .UseAuthentication()
    .UseAuthorization()
    .UseUserContext()
    .UseSupportMiddleware();

app.MapCarter()
    .MapStoreItemsEndpoints()
    .MapAccountsEndpoints()
    .MapDashboardEndpoints()
    .MapCashflowsEndpoints()
    .MapTransactionsEndpoints()
    .MapSettingsEndpoints()
    .MapCategoriesEndpoints()
    .MapHomeEndpoints();

if (!builder.Environment.IsEnvironment("SECURITY_TESTS"))
{
    app.UseHangfire(builder.Configuration);
    app.UseHealthChecks();
    app.MigrateDb();
}

await app.RunAsync();
