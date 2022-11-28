using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using Holefeeder.Application.Behaviors;
using Holefeeder.Application.Context;
using Holefeeder.Application.Features.MyData.Commands;
using Holefeeder.Application.SeedWork;
using Holefeeder.Application.SeedWork.BackgroundRequest;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Holefeeder.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var storeItemConnectionString = configuration.GetConnectionString("ObjectStoreConnectionString");
        services.AddDbContext<StoreItemContext>(options =>
        {
            options.UseMySql(storeItemConnectionString, ServerVersion.AutoDetect(storeItemConnectionString));
        });

        var holefeederConnectionString = configuration.GetConnectionString("HolefeederConnectionString");
        services.AddDbContext<BudgetingContext>(options =>
        {
            options.UseMySql(holefeederConnectionString, ServerVersion.AutoDetect(holefeederConnectionString));
        });

        services.AddHttpContextAccessor();
        services.AddTransient<IUserContext, UserContext>();

        // For all the validators, register them with dependency injection as scoped
        AssemblyScanner
            .FindValidatorsInAssembly(typeof(Application).Assembly, true)
            .ForEach(item => services.AddTransient(item.InterfaceType, item.ValidatorType));

        services.AddMemoryCache();

        services
            .AddMediatR(typeof(Application).Assembly)
            .AddSingleton<BackgroundWorkers>()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>))
            .AddTransient<ImportData.BackgroundTask>();

        //services.AddGraphQLServer().AddQueryType<Query>().AddProjections().AddFiltering().AddSorting();

        return services;
    }
}
