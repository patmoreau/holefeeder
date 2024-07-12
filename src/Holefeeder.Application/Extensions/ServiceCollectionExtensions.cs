using System.Diagnostics.CodeAnalysis;

using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Mediatr;
using DrifterApps.Seeds.Domain;

using Holefeeder.Application.Authorization;
using Holefeeder.Application.Context;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Holefeeder.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddUserContext();

        // For all the validators, register them with dependency injection as scoped
        AssemblyScanner
            .FindValidatorsInAssembly(typeof(Application).Assembly, true)
            .ForEach(item => services.AddTransient(item.InterfaceType, item.ValidatorType));

        services.AddMemoryCache();

        services
            .AddMediatR(serviceConfiguration =>
                serviceConfiguration.RegisterServicesFromAssembly(typeof(Application).Assembly)
                    .RegisterServicesFromApplicationSeeds());

        services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
        services.AddTransient<IUnitOfWork>(provider => provider.GetRequiredService<BudgetingContext>());
        services.AddScoped<IUserContext, UserContext.UserContext>();

        return services;
    }
}
