using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using DrifterApps.Holefeeder.API.Authorization;
using DrifterApps.Holefeeder.API.Authorization.Google;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Queries;
using DrifterApps.Holefeeder.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NJsonSchema.Generation;
using NSwag.AspNetCore;

namespace DrifterApps.Holefeeder.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            _ = RegisterServices(services);
            
            var googleClientId = Configuration["Authentication:Google:ClientId"];
            if (string.IsNullOrWhiteSpace(googleClientId))
                throw new NullReferenceException(@"Google Client Id not configured");
            var googleAuthority = Configuration["Authentication:Google:Authority"];
            if (string.IsNullOrWhiteSpace(googleAuthority))
                throw new NullReferenceException(@"Google Authority not configured");

            var allowedOrigins = Configuration.GetSection("AllowedHosts")?.Get<string[]>();
            if (allowedOrigins == null || !allowedOrigins.Any())
                throw new NullReferenceException(@"Allowed origin values not configured");

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

            services.AddControllers();

            _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.UseGoogle(googleClientId, googleAuthority);
                    x.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var mediator = context.HttpContext.RequestServices.GetService<IMediator>();

                            var user = await mediator.Send(new GetUserQuery(context.Principal.FindFirstValue(JwtRegisteredClaimNames.Email)));
                            
                            if (user != null)
                            {
                                context.Principal.AddIdentity(new ClaimsIdentity(new[]
                                {
                                    new Claim(HolefeederClaimTypes.HOLEFEEDER_ID, user.Id.ToString())
                                }));
                            }
                        }
                    };
                });

            services.AddSwaggerDocument();

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
                    })
                ;
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _ = env.IsDevelopment() ? app.UseDeveloperExceptionPage() : app.UseHsts();

            app.UseHttpsRedirection();

            app.UseRouting()
                .UseCors()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }

        private IServiceCollection RegisterServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(GetAccountsHandler).Assembly);
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddHolefeederDatabase(Configuration);

            return services;
        }
    }
}
