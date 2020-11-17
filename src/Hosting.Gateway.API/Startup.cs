using System;
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

namespace DrifterApps.Holefeeder.Hosting.Gateway.API
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var allowedOrigin = Configuration["AllowedHosts"];
            if (string.IsNullOrWhiteSpace(allowedOrigin))
                throw new NullReferenceException(@"Allowed origin values not configured");

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins(allowedOrigin)
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
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseMvc();
            app.UseCors();
            
            app.UseSerilogRequestLogging()
                // .UseRouting()
                // .UseEndpoints(endpoints =>
                // {
                //     endpoints.MapControllers();
                // })
                ;
            await app.UseOcelot();
        }
    }
}
