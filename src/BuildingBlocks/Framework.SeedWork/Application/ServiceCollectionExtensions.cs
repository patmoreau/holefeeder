using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services, Assembly assembly,
        Action<(Type ValidatorType, Type InterfaceType)> process)
    {
        var types = from x in assembly.GetTypes()
            from z in x.GetInterfaces()
            where z.IsGenericType && typeof(IValidator<,>).IsAssignableFrom(z.GetGenericTypeDefinition())
            select (ValidatorType: x, InterfaceType: z);
        
        foreach (var type in types)
        {
            process(type);
        }

        return services;
    }
}
