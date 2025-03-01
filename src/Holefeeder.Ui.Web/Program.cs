using Holefeeder.Ui.Shared.Authentication;
using Holefeeder.Ui.Shared.Extensions;
using Holefeeder.Ui.Shared.Services;
using Holefeeder.Ui.Web;
using Holefeeder.Ui.Web.Authentication;
using Holefeeder.Ui.Web.Services;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;

using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.Configure<Auth0Options>(builder.Configuration.GetSection(Auth0Options.Section));
builder.Services.Configure<DownStreamApiOptions>(builder.Configuration.GetSection(DownStreamApiOptions.Section));

builder.Services.AddScoped<HttpClient>(provider =>
{
    var auth0Config = provider.GetRequiredService<IOptions<Auth0Options>>().Value;
    return new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)};
});

builder.Services.AddOidcAuthentication(options =>
{
    using var provider = builder.Services.BuildServiceProvider();
    var auth0Config = provider.GetRequiredService<IOptions<Auth0Options>>().Value;
    var downStreamApiConfig = provider.GetRequiredService<IOptions<DownStreamApiOptions>>().Value;

    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", auth0Config.Audience);
    foreach (var scope in downStreamApiConfig.ScopesArray)
    {
        options.ProviderOptions.DefaultScopes.Add(scope);
    }
});
builder.Services.AddMudServices();
builder.Services.AddScoped<AuthenticationMessageHandler>();
builder.Services.AddScoped<IAuthNavigationManager, AuthNavigationManager>();
builder.Services.AddRefitClients(b =>
    b.AddHttpMessageHandler(provider => provider.GetRequiredService<AuthenticationMessageHandler>()));
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();
