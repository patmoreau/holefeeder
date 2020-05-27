using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text.Json;
using DrifterApps.Holefeeder.Business;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common.Authorization;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.Common.IoC;
using DrifterApps.Holefeeder.ServicesHosts.BudgetApi.Authentication.Google;
using DrifterApps.Holefeeder.ServicesHosts.BudgetApi.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Win32.SafeHandles;
using SimpleInjector;

namespace DrifterApps.Holefeeder.ServicesHosts.BudgetApi
{
    public sealed class Startup : IDisposable
    {
        private readonly Container _container;

        // Flag: Has Dispose already been called?
        private bool _disposed;

        // Instantiate a SafeHandle instance.
        private readonly SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

        public Startup(IConfiguration configuration)
        {
            _container = new Container();
            _container.Options.ResolveUnregisteredConcreteTypes = false;

            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var googleClientId = Configuration["Authentication:Google:ClientId"];
            if(string.IsNullOrWhiteSpace(googleClientId))
                throw new NullReferenceException(BudgetApiInternal.GoogleClientIdNotConfigured);
            var googleAuthority = Configuration["Authentication:Google:Authority"];
            if(string.IsNullOrWhiteSpace(googleAuthority))
                throw new NullReferenceException(BudgetApiInternal.GoogleAuthorityNotConfigured);

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
                });

            _ = services.AddLogging();

            _ = services.AddSimpleInjector(_container, options =>
              {
                  options.AddLogging()
                      .AddAspNetCore()
                      .AddControllerActivation();
              });

            _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.UseGoogle(googleClientId, googleAuthority);
                    x.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var usersServices = _container.GetService<IUsersService>();
            
                            var user = await usersServices.FindByEmailAsync(context.Principal.FindFirstValue(JwtRegisteredClaimNames.Email)).ConfigureAwait(false) ??
                                       await usersServices.CreateAsync(new UserEntity(
                                null,
                                context.Principal.FindFirstValue(JwtRegisteredClaimNames.GivenName),
                                context.Principal.FindFirstValue(JwtRegisteredClaimNames.FamilyName),
                                context.Principal.FindFirstValue(JwtRegisteredClaimNames.Email),
                                context.Principal.FindFirstValue(JwtRegisteredClaimNames.Sub),
                                DateTime.Now.Date

                            )).ConfigureAwait(false);

                            if (user != null)
                            {
                                context.Principal.AddIdentity(new ClaimsIdentity(new[]
                                {
                                    new Claim(HolefeederClaimTypes.HOLEFEEDER_ID, user.Id)
                                }));
                            }
                        }
                    };
                });

            _ = services.AddSwaggerGen(opt =>
              {
                  opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Holefeeder API", Version = "v1" });
                  opt.DescribeAllParametersInCamelCase();
              });

            _container.Initialize(Configuration);

            _ = services
                .AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddApiExplorer()
                .AddAuthorization(options =>
                {
                    options.AddPolicy(Policies.AUTHENTICATED_USER, policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser()
                            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                            .RequireClaim(JwtRegisteredClaimNames.Email)
                            .RequireClaim("email_verified", "true");
                    });
                    options.AddPolicy(Policies.REGISTERED_USER, policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser()
                            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                            .RequireClaim(JwtRegisteredClaimNames.Email)
                            .RequireClaim("email_verified", "true")
                            .RequireClaim(HolefeederClaimTypes.HOLEFEEDER_ID);
                    });
                });
        }

        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _ = app.UseSimpleInjector(_container);

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

            _ = app.UseRouting()
                .UseCors()
                .UseAuthorization()
                .UseAuthentication()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            _container.Verify();
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
                _container.Dispose();
            }

            _disposed = true;
        }
    }
}
