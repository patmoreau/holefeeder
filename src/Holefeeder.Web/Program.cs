using Carter;

using HealthChecks.UI.Client;

using Holefeeder.Web.Config;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

builder.Services.AddHttpLogging(_ => { });
builder.Services.AddControllersWithViews();
builder.Services.Configure<AngularSettings>(builder.Configuration.GetSection(nameof(AngularSettings)))
    .AddSingleton(sp => sp.GetRequiredService<IOptions<AngularSettings>>().Value);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAdB2C");

var apiUri = builder.Configuration.GetValue<string>("Api:Url") ??
             throw new InvalidOperationException("Missing `Api:Url` configuration");

builder.Services
    .AddHealthChecksUI(setup =>
    {
        setup.AddHealthCheckEndpoint("hc-web", "/healthz");
        setup.AddHealthCheckEndpoint("hc-api", $"{apiUri}/healthz");
    })
    .AddInMemoryStorage();

builder.Services
    .AddCarter(configurator: configurator => configurator.WithEmptyValidators());

var tags = new[] { "holefeeder", "web", "service" };
builder.Services.AddHealthChecks().AddCheck("web", () => HealthCheckResult.Healthy(), tags);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("customPolicy", policy => policy.RequireAuthenticatedUser());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
else
{
    IdentityModelEventSource.ShowPII = true;
}

app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();

app.MapFallbackToFile("index.html");

app.MapHealthChecks("/healthz",
    new HealthCheckOptions { Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse }
);
app.UseHttpLogging();
app.MapHealthChecksUI(config =>
{
    config.UIPath = "/hc-ui";
    config.ResourcesPath = "/hc-ui/resources";
    config.ApiPath = "/hc-ui/hc-api";
    config.WebhookPath = "/hc-ui/webhooks";
    config.UseRelativeApiPath = true;
    config.UseRelativeResourcesPath = true;
    config.UseRelativeWebhookPath = true;
});

await app.RunAsync();
