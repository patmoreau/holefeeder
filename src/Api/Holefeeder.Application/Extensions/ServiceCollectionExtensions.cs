using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using Holefeeder.Application.Behaviors;
using Holefeeder.Application.Features.MyData.Commands;
using Holefeeder.Application.SeedWork;
using Holefeeder.Application.SeedWork.BackgroundRequest;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Holefeeder.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IUserContext, UserContext>();

        // For all the validators, register them with dependency injection as scoped
        AssemblyScanner
            .FindValidatorsInAssembly(typeof(Application).Assembly)
            .ForEach(item => services.AddTransient(item.InterfaceType, item.ValidatorType));

        services.AddMemoryCache();

        services
            .AddMediatR(typeof(Application).Assembly)
            .AddSingleton<BackgroundWorkerQueue>()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddTransient<ImportData.BackgroundTask>();

        return services;
    }
}
