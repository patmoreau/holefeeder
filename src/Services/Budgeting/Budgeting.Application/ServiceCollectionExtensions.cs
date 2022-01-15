using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Queries;
using DrifterApps.Holefeeder.Budgeting.Application.Behaviors;
using DrifterApps.Holefeeder.Budgeting.Application.MyData.Commands;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Application.BackgroundRequest;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Holefeeder.Budgeting.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // For all the validators, register them with dependency injection as scoped
        AssemblyScanner.FindValidatorsInAssembly(typeof(GetAccounts).Assembly)
            .ForEach(item => services.AddTransient(item.InterfaceType, item.ValidatorType));
            
        services.AddMemoryCache();
            
        services.AddMediatR(typeof(GetAccounts).Assembly)
            .AddSingleton<BackgroundWorkerQueue>()
            .AddTransient<ImportData.BackgroundTask>()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddValidators(typeof(GetAccounts).Assembly,
                item => services.AddScoped(item.InterfaceType, item.ValidatorType));

        services.AddScoped<ItemsCache>();

        return services;
    }
}
