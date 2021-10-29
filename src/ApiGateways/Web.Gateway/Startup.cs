using System.Linq;

using DrifterApps.Holefeeder.Web.Gateway.Ocelot;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
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

namespace DrifterApps.Holefeeder.Web.Gateway
{
    public class Startup
    {
        private const string MY_ALLOW_SPECIFIC_ORIGINS = "_myAllowSpecificOrigins";

        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var allowedOrigins = Configuration.GetValue<string>("AllowedHosts").Split(";").ToArray();
            services.AddCors(options =>
            {
                options.AddPolicy(MY_ALLOW_SPECIFIC_ORIGINS, builder =>
                {
                    builder
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("X-Total-Count");
                });
            });
            
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());
 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(
                    options => options.TokenValidationParameters =
                        new TokenValidationParameters {ValidateIssuer = false},
                    options => Configuration.Bind("AzureAd", options));
            
            services.AddMvcCore(options => options.EnableEndpointRouting = false);
            
            services.AddOcelot()
                .AddCacheManager(x => x.WithDictionaryHandle());
            
            services.ConfigureDownstreamHostAndPortsPlaceholders(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            
            app.UseCors(MY_ALLOW_SPECIFIC_ORIGINS);

            app.UseRouting();

            app.UseSerilogRequestLogging();
                        
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });

            app.UseOcelot().GetAwaiter().GetResult();
        }
    }
}
