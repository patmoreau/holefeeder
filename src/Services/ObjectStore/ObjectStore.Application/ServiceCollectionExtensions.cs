using DrifterApps.Holefeeder.ObjectStore.Application.Behaviors;
using DrifterApps.Holefeeder.ObjectStore.Application.Queries;
using DrifterApps.Holefeeder.ObjectStore.Application.Validators;

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
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

        AssemblyScanner.FindValidatorsInAssembly(typeof(CreateStoreItemCommandValidator).Assembly)
            .ForEach(item => services.AddScoped(item.InterfaceType, item.ValidatorType));

        services.AddScoped<ItemsCache>();

        return services;
    }
}
