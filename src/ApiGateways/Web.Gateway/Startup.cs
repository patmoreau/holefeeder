using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(
                    options => options.TokenValidationParameters =
                        new TokenValidationParameters {ValidateIssuer = false},
                    options => Configuration.Bind("AzureAd", options));
            
            services.AddMvcCore(options => options.EnableEndpointRouting = false);
            services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            
            app.UseCors(MY_ALLOW_SPECIFIC_ORIGINS);
            
            app.UseSerilogRequestLogging();
            await app.UseOcelot();
        }
    }
}
