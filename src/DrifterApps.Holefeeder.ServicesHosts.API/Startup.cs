using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using DrifterApps.Holefeeder.Business;
using DrifterApps.Holefeeder.Common.Authorization;
using DrifterApps.Holefeeder.Common.IoC;
using Holefeeder.Services.API.Authentication;
using Holefeeder.Services.API.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SimpleInjector;

namespace DrifterApps.Holefeeder.Hosts.API
{
    public class Startup
    {
        private const string MY_ALLOW_SPECIFIC_ORIGINS = "_myAllowSpecificOrigins";

        private readonly Container _container;

        public Startup(IConfiguration configuration)
        {
            _container = new Container();
            _container.Options.ResolveUnregisteredConcreteTypes = false;

            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
                    x.UseGoogle(clientId: "890828371465-fdjn2okrids500dqcogjkogbca2i2929.apps.googleusercontent.com");
                    x.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var usersServices = _container.GetService<IUsersService>();

                            var user = await usersServices.FindByEmailAsync(context.Principal.FindFirstValue(JwtRegisteredClaimNames.Email));
                            if(user == null)
                            {
                                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                                context.Fail("User is not registered");
                            }
                            if(user != null)
                            {
                                context.Principal.AddIdentity(new ClaimsIdentity(new[]
                                {
                                    new Claim(HolefeederClaimTypes.HolefeederId, user.Id)
                                }));
                            }
                        }
                    };
                });

            _ = services.AddCors(options =>
            {
                options.AddPolicy(MY_ALLOW_SPECIFIC_ORIGINS, builder =>
                {
                    builder
                        // .AllowAnyOrigin()
                        .WithOrigins("https://holefeeder.drifterapps.com", "http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("X-Total-Count")
                        // .AllowCredentials()
                        ;
                });
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
                .AddAuthorization((options) =>
                {
                    options.AddPolicy(Policies.AuthenticatedUser, policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser()
                            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                            .RequireClaim(JwtRegisteredClaimNames.Email)
                            .RequireClaim("email_verified", "true");
                    });
                    options.AddPolicy(Policies.RegisteredUser, policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser()
                            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                            .RequireClaim(JwtRegisteredClaimNames.Email)
                            .RequireClaim("email_verified", "true")
                            .RequireClaim(HolefeederClaimTypes.HolefeederId);
                    });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _ = app.UseSimpleInjector(_container);

            if (env.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage()
                    .UseSwagger()
                    .UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Holefeeder API");
                        c.RoutePrefix = string.Empty;
                    });
            }
            else
            {
                _ = app.UseHsts();
            }

            _ = app.UseHttpsRedirection()
                .UseRouting()
                .UseAuthorization()
                .UseAuthentication()
                .UseCors(MY_ALLOW_SPECIFIC_ORIGINS)
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            _container.Verify();
        }
    }
}