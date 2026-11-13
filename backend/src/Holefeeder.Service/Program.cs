using Dapper;

using Holefeeder.Service;
using Holefeeder.Service.Accounts;
using Holefeeder.Service.Cashflows;
using Holefeeder.Service.Categories;
using Holefeeder.Service.Dashboard;
using Holefeeder.Service.ErrorHandling;
using Holefeeder.Service.Extensions;
using Holefeeder.Service.Home;
using Holefeeder.Service.Insights;
using Holefeeder.Service.MyData;
using Holefeeder.Service.Settings;
using Holefeeder.Service.StoreItems;
using Holefeeder.Service.Support;
using Holefeeder.Service.Support.Persistence.Schema;
using Holefeeder.Service.Sync;
using Holefeeder.Service.Tags;
using Holefeeder.Service.Transactions;
using Holefeeder.Service.Users;

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

builder.Services.AddHangfireServices(DatabaseConnection.StringFrom);
builder.Services.AddHealthChecks(builder.Configuration);

builder.Services
    .AddSingleton(provider => NpgsqlDataSource.Create(DatabaseConnection.StringFrom(provider)))
    .AddStoreItemsServices()
    .AddAccountsServices()
    .AddDashboardServices()
    .AddCashflowsServices()
    .AddTagsServices()
    .AddInsightsServices()
    .AddTransactionsServices()
    .AddSettingsServices()
    .AddCategoriesServices()
    .AddUsersServices()
    .AddSyncServices()
    .AddMyDataServices()
    .AddSupportServices();

builder.Services.AddOptions<ScalarOptions>().BindConfiguration("Scalar");

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
    .UseSupportMiddleware();

app
    .MapStoreItemsEndpoints()
    .MapAccountsEndpoints()
    .MapDashboardEndpoints()
    .MapCashflowsEndpoints()
    .MapTagsEndpoints()
    .MapInsightsEndpoints()
    .MapTransactionsEndpoints()
    .MapSettingsEndpoints()
    .MapCategoriesEndpoints()
    .MapUsersEndpoints()
    .MapSyncEndpoints()
    .MapMyDataEndpoints()
    .MapHomeEndpoints();

app.UseHealthChecks();
app.MigrateDb(DatabaseConnection.StringFrom(app.Services));

await app.RunAsync();
