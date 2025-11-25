using Carter;


using Holefeeder.Web.Config;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllersWithViews();
builder.Services.Configure<AngularSettings>(builder.Configuration.GetSection(nameof(AngularSettings)))
    .AddSingleton(sp => sp.GetRequiredService<IOptions<AngularSettings>>().Value);

var apiUri = builder.Configuration.GetValue<string>("Api:Url") ??
             throw new InvalidOperationException("Missing `Api:Url` configuration");

builder.Services
    .AddCarter(configurator: configurator => configurator.WithEmptyValidators());

var tags = new[] { "holefeeder", "web", "service" };
builder.Services.AddHealthChecks().AddCheck("web", () => HealthCheckResult.Healthy(), tags);

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

app.MapCarter();

app.MapHealthChecks("/healthz",
    new HealthCheckOptions { Predicate = _ => true }
);

app.MapFallbackToFile("index.html");

await app.RunAsync();
