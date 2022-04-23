using DrifterApps.Holefeeder.Budgeting.Application.Behaviors;
using DrifterApps.Holefeeder.Budgeting.Application.MyData.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions.Queries;
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
        AssemblyScanner.FindValidatorsInAssembly(typeof(GetTransactions).Assembly)
            .ForEach(item => services.AddTransient(item.InterfaceType, item.ValidatorType));

        services.AddMemoryCache();

        services.AddMediatR(typeof(GetTransactions).Assembly)
            .AddSingleton<BackgroundWorkerQueue>()
            .AddTransient<ImportData.BackgroundTask>()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddValidators(typeof(GetTransactions).Assembly,
                item => services.AddScoped(item.InterfaceType, item.ValidatorType));

        services.AddScoped<ItemsCache>();

        return services;
    }
}
