using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.Infrastructure.Context;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using SolidToken.SpecFlow.DependencyInjection;

namespace Holefeeder.FunctionalTests.Hooks;

public static class DependencyInjectionHook
{
    // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks

    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();

        IConfigurationBuilder configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonFile("appsettings.tests.json", false);
        IConfiguration configuration = configBuilder.Build();

        services.AddHttpClient();

        services.AddOptions<ObjectStoreDatabaseSettings>()
            .Bind(configuration.GetSection(nameof(ObjectStoreDatabaseSettings)));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<ObjectStoreDatabaseSettings>>().Value);

        services.AddSingleton<DatabaseDriver>();

        return services;
    }
}
