using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.IoC;
using DrifterApps.Holefeeder.ServicesHosts.BudgetApi.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Win32.SafeHandles;
using Serilog;

namespace DrifterApps.Holefeeder.ServicesHosts.BudgetApi
{
    public sealed class Startup : IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool _disposed;

        // Instantiate a SafeHandle instance.
        private readonly SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var allowedOrigins = Configuration.GetSection("AllowedOrigin")?.Get<string[]>();
            if (allowedOrigins == null || !allowedOrigins.Any())
                throw new NullReferenceException(BudgetApiInternal.AllowedOriginNotConfigured);

            _ = services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("X-Total-Count");
                });
            });

            _ = services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            _ = services.AddLogging();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                
                .AddMicrosoftIdentityWebApi(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false
                    };
                }, options =>
                {
                    Configuration.Bind("AzureAd", options);
                });

            _ = services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo {Title = "Holefeeder API", Version = "v1"});
                opt.DescribeAllParametersInCamelCase();
            });

            services.Initialize(Configuration);

            _ = services
                .AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddApiExplorer()
                .AddAuthorization(options => 
                {
                    options.AddPolicy("registered_users", policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser()
                            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    });
                });
        }

        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage()
                    .UseSwagger()
                    .UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Holefeeder Budget Api");
                        c.RoutePrefix = string.Empty;
                    });
            }
            else
            {
                _ = app.UseHsts();
            }
            
            app.UseSerilogRequestLogging();
            
            _ = app.UseRouting()
                .UseCors()
                .UseAuthorization()
                .UseAuthentication()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _handle.Dispose();
            }

            _disposed = true;
        }
    }
}
