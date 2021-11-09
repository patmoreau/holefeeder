using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Queries;
using DrifterApps.Holefeeder.Budgeting.Application.Behaviors;
using DrifterApps.Holefeeder.Budgeting.Application.Imports.Commands;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Holefeeder.Budgeting.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // For all the validators, register them with dependency injection as scoped
            AssemblyScanner.FindValidatorsInAssembly(typeof(GetAccountsHandler).Assembly)
                .ForEach(item => services.AddTransient(item.InterfaceType, item.ValidatorType));
            
            services.AddMemoryCache();
            
            services.AddMediatR(typeof(GetAccountsHandler).Assembly)
                .AddTransient<ImportDataCommandTask>()
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

            return services;
        }
    }
}
