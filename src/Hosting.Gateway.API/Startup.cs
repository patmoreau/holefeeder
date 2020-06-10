using System.Collections.Generic;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Users.Queries;
using DrifterApps.Holefeeder.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

namespace DrifterApps.Holefeeder.Hosting.OcelotGateway.API
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
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(host => true)
                        .AllowCredentials());
            });

            // services.AddMicrosoftIdentityWebApiAuthentication(Configuration);
            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //     .AddMicrosoftIdentityWebApi(options =>
            //     {
            //         options.TokenValidationParameters = new TokenValidationParameters {ValidateIssuer = false};
            //         options.Events = new JwtBearerEvents
            //         {
            //             OnAuthenticationFailed = context =>
            //             {
            //                 return Task.CompletedTask;
            //             },
            //             OnTokenValidated = context =>
            //             {
            //                 return Task.CompletedTask;
            //             }
            //         };
            //     }, options =>
            //     {
            //         Configuration.Bind("AzureAd", options);
            //         // options.TokenValidationParameters = new TokenValidationParameters
            //         // {
            //         //     ValidIssuers = new List<string>
            //         //     {
            //         //         $"https://login.microsoftonline.com/{Configuration["AzureAd:Domain"]}/{Configuration["AzureAd:SignUpSignInPolicyId"]}/v2.0/",
            //         //         $"https://holefeeder.b2clogin.com/{Configuration["AzureAd:Domain"]}/{Configuration["AzureAd:SignUpSignInPolicyId"]}/v2.0/"
            //         //     }
            //         // };
            //     });

            _ = RegisterServices(services);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("registered_users", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                });
            });

            services.AddOcelot(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // var pathBase = Configuration["PATH_BASE"];
            //
            // if (!string.IsNullOrEmpty(pathBase))
            // {
            //     app.UsePathBase(pathBase);
            // }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();
            // app.UseRouting()
            //     .UseCors("CorsPolicy")
            //     .UseAuthorization()
            //     .UseAuthentication();
            app.UseCors("CorsPolicy");

            app.UseOcelot().Wait();
        }

        private IServiceCollection RegisterServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(GetUserHandler).Assembly);

            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddHolefeederDatabase(Configuration);

            return services;
        }
    }
}
