using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Users.Queries;
using DrifterApps.Holefeeder.Hosting.OcelotGateway.API.Authorization.Google;
using DrifterApps.Holefeeder.Hosting.Security;
using DrifterApps.Holefeeder.Infrastructure.Database;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityModel.AspNetCore.OAuth2Introspection;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

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
            _ = RegisterServices(services);

            //var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            // const string authenticationProviderKey = "Bearer";
            IdentityModelEventSource.ShowPII = true;
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(host => true)
                        .AllowCredentials());
            });

            // var googleClientSecret = "xBZtkWK_07w5iUux2F3mpi9D";
            // var googleClientId = "890828371465-fdjn2okrids500dqcogjkogbca2i2929.apps.googleusercontent.com";// Configuration["Authentication:Google:ClientId"];
            // if (string.IsNullOrWhiteSpace(googleClientId))
            //     throw new NullReferenceException(@"Google Client Id not configured");
            // var googleAuthority = "https://accounts.google.com"; // Configuration["Authentication:Google:Authority"];
            // if (string.IsNullOrWhiteSpace(googleAuthority))
            //     throw new NullReferenceException(@"Google Authority not configured");

            // _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //     .AddJwtBearer(x =>
            //     {
            //         x.UseGoogle(googleClientId, googleAuthority);
            //         x.Events = new JwtBearerEvents
            //         {
            //             OnTokenValidated = async context =>
            //             {
            //                 var mediator = context.HttpContext.RequestServices.GetService<IMediator>();
            //
            //                 var user = await mediator.Send(new GetUserQuery(context.Principal.FindFirstValue(JwtRegisteredClaimNames.Email)));
            //                 
            //                 if (user != null)
            //                 {
            //                     context.Principal.AddIdentity(new ClaimsIdentity(new[]
            //                     {
            //                         new Claim(HolefeederClaimTypes.HOLEFEEDER_ID, user.Id.ToString())
            //                     }));
            //                 }
            //             }
            //         };
            //     });

            services.AddDistributedMemoryCache();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            // this API will accept any access token from the authority
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.TokenValidationParameters.ValidateAudience = false;
                    
                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                    // if token does not contain a dot, it is a reference token
                    options.ForwardDefaultSelector = Selector.ForwardReferenceToken("introspection");
                    options.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = async context =>
                        {
                            await Task.Run(() => _ = 0);
                        },
                        OnTokenValidated = async context =>
                        {
                            var id = context.Principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
                            await Task.Run(() => _ = 0);
                            // var mediator = context.HttpContext.RequestServices.GetService<IMediator>();
                            //
                            // var user = await mediator.Send(new GetUserQuery(context.Principal.FindFirstValue(JwtRegisteredClaimNames.Email)));
                            //
                            // if (user != null)
                            // {
                            //     context.Principal.AddIdentity(new ClaimsIdentity(new[]
                            //     {
                            //         new Claim(HolefeederClaimTypes.HOLEFEEDER_ID, user.Id.ToString())
                            //     }));
                            // }
                        },
                        OnMessageReceived = async context =>
                        {
                            await Task.Run(() => _ = 0);
                        }
                    };
                })
                // reference tokens
                .AddOAuth2Introspection("introspection", options =>
                {
                    options.Authority = "https://localhost:5001";

                    options.ClientId = "interactive";
                    options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
                    options.Events = new OAuth2IntrospectionEvents
                    {
                        OnAuthenticationFailed = async context =>
                        {
                            await Task.Run(() => _ = 0);
                        },
                        OnTokenValidated = async context =>
                        {
                            await Task.Run(() => _ = 0);
                        }
                    };
                });;
            services.AddScopeTransformation();
            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //     .AddGoogle(JwtBearerDefaults.AuthenticationScheme, options =>
            //     {
            //         options.ClientId = googleClientId;
            //         options.ClientSecret = googleClientSecret;
            //         options.Events = new OAuthEvents()
            //         {
            //             OnAccessDenied = async context =>
            //             {
            //                 await Task.Run(() => _ = 0);
            //             },
            //             OnTicketReceived = async context =>
            //             {
            //                 await Task.Run(() => _ = 0);
            //             }
            //         };
            //     });
                // .AddJwtBearer(authenticationProviderKey, x =>
                // {
                //     x.UseGoogle(googleClientId, googleAuthority);
                //     // x.Authority = googleAuthority;
                //     // x.Audience = googleClientId;
                //     x.Events = new JwtBearerEvents()
                //     {
                //         OnAuthenticationFailed = async context =>
                //         {
                //             await Task.Run(() => _ = 0);
                //         },
                //         OnTokenValidated = async context =>
                //         {
                //             var mediator = context.HttpContext.RequestServices.GetService<IMediator>();
                //             
                //             var user = await mediator.Send(new GetUserQuery(context.Principal.FindFirstValue(JwtRegisteredClaimNames.Email)));
                //             
                //             if (user != null)
                //             {
                //                 context.Principal.AddIdentity(new ClaimsIdentity(new[]
                //                 {
                //                     new Claim(HolefeederClaimTypes.HOLEFEEDER_ID, user.Id.ToString())
                //                 }));
                //             }
                //         },
                //         OnMessageReceived = async context =>
                //         {
                //             await Task.Run(() => _ = 0);
                //         }
                //     };
                // });

            services.AddOcelot(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var pathBase = Configuration["PATH_BASE"];

            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
