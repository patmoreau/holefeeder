using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Behaviors;
using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Queries;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Holefeeder.ObjectStore.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(GetStoreItemHandler).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        AssemblyScanner.FindValidatorsInAssembly(typeof(ServiceCollectionExtensions).Assembly)
            .ForEach(item => services.AddScoped(item.InterfaceType, item.ValidatorType));

        services.AddScoped<ItemsCache>();

        return services;
    }
}
