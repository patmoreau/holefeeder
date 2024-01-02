using Carter;

using HealthChecks.UI.Client;

using Holefeeder.Web.Config;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

builder.Services.AddHttpLogging(_ => { });
builder.Services.AddControllersWithViews();
builder.Services.Configure<AngularSettings>(builder.Configuration.GetSection(nameof(AngularSettings)))
    .AddSingleton(sp => sp.GetRequiredService<IOptions<AngularSettings>>().Value);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(
        options => options.TokenValidationParameters =
            new TokenValidationParameters { ValidateIssuer = true },
        options => builder.Configuration.Bind("AzureAdB2C", options));

var apiUri = builder.Configuration.GetValue<string>("Api:Url") ??
             throw new InvalidOperationException("Missing `Api:Url` configuration");

builder.Services
    .AddHealthChecksUI(setup =>
    {
        setup.AddHealthCheckEndpoint("web", "http://localhost/healthz");
        setup.AddHealthCheckEndpoint("api", $"{apiUri}/healthz");
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
    config.ApiPath = "/hc-ui/api";
    config.WebhookPath = "/hc-ui/webhooks";
    config.UseRelativeApiPath = true;
    config.UseRelativeResourcesPath = true;
    config.UseRelativeWebhookPath = true;
});

app.Run();
