using System.Linq;

using DrifterApps.Holefeeder.Web.Gateway.Ocelot;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

using Serilog;

const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetValue<string>("AllowedHosts").Split(";").ToArray();

builder.Services
  .AddCors(options =>
  {
    options.AddPolicy(myAllowSpecificOrigins, policyBuilder =>
    {
      policyBuilder
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("X-Total-Count");
    });
  })
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddMicrosoftIdentityWebApi(
    options => options.TokenValidationParameters =
      new TokenValidationParameters {ValidateIssuer = false},
    options => builder.Configuration.Bind("AzureAd", options));

builder.Services
  .AddMvcCore(options => options.EnableEndpointRouting = false);

builder.Services
  .AddOcelot()
  .AddCacheManager(x => x.WithDictionaryHandle());

builder.Services
  .ConfigureDownstreamHostAndPortsPlaceholders(builder.Configuration)
  .AddHealthChecks()
  .AddCheck("self", () => HealthCheckResult.Healthy());

builder.Host.UseSerilog();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}
else
{
  app.UseExceptionHandler("/Error")
    .UseHsts();
}

app.UseMvc()
  .UseCors(myAllowSpecificOrigins)
  .UseRouting()
  .UseSerilogRequestLogging()
  .UseEndpoints(endpoints =>
{
  endpoints.MapHealthChecks("/hc",
    new HealthCheckOptions {ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse});
})
  .UseOcelot().GetAwaiter().GetResult();

app.Run();

namespace DrifterApps.Holefeeder.Budgeting.API
{
  public class Program
  {
  }
}
