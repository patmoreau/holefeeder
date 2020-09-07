using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using DrifterApps.Holefeeder.API.Authorization;
using DrifterApps.Holefeeder.API.Authorization.Google;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Queries;
using DrifterApps.Holefeeder.Infrastructure.MongoDB;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Context;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

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
            services.AddMediatR(typeof(GetAccountsHandler).Assembly);
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddScoped<IAccountQueriesRepository, AccountsQueriesRepository>();
            services.AddScoped<IUserQueriesRepository, UserQueriesRepository>();

            var mongoConfig = Configuration.GetSection("MongoDB");
            services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConfig["ConnectionString"]));
            services.AddScoped(provider =>
                provider.GetService<IMongoClient>().GetDatabase(mongoConfig["DatabaseName"]));
            services.AddTransient<IMongoDbContext, MongoDbContext>();

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

                            var user = await mediator.Send(new GetUserRequest(context.Principal.FindFirstValue(JwtRegisteredClaimNames.Email)));
                            
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
    }
}
