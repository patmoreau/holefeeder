using Holefeeder.Ui.Shared.Authentication;
using Holefeeder.Ui.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace Holefeeder.Ui.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTokenService(this IServiceCollection services)
    {
        services.AddScoped<TokenService>();
        services.AddSingleton<ExceptionService>();

        return services;
    }

    public static IServiceCollection AddRefitClients(this IServiceCollection services,
        Func<IHttpClientBuilder, IHttpClientBuilder>? configureClient = null)
    {
        var builder = services.AddRefitClient<IHolefeederApiService>(new RefitSettings
            {
                UrlParameterFormatter = new CustomDateUrlParameterFormatter()
            })
            .ConfigureHttpClient((provider, client) =>
            {
                var apiConfig = provider.GetRequiredService<IOptions<DownStreamApiOptions>>().Value;
                client.BaseAddress = apiConfig.BaseUrl;
            });
        configureClient?.Invoke(builder);

        return services;
    }
}
