using System.Text.Json;

using Carter;

using DrifterApps.Seeds.Application.Converters;
using DrifterApps.Seeds.Infrastructure;

using Holefeeder.Api.ErrorHandling;
using Holefeeder.Api.Extensions;
using Holefeeder.Application.Converters;
using Holefeeder.Application.Extensions;
using Holefeeder.Infrastructure.Extensions;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;

using Scalar.AspNetCore;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddCors(builder.Configuration)
    .AddSecurity(builder.Configuration)
    .AddCarter(configurator: configurator => configurator.WithEmptyValidators())
    .AddOpenApi("v2", options =>
    {
        options.AddBearerTokenAuthentication();
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info.Contact = new OpenApiContact
            {
                Name = "Drifter Apps Inc.",
                Email = "info@drifterapps.com"
            };
            return Task.CompletedTask;
        });
    });

if (!builder.Environment.IsEnvironment("SECURITY_TESTS"))
{
    builder.Services.AddHangfireServices();
    builder.Services.AddHealthChecks(builder.Configuration);
}

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddHangfireRequestScheduler(() =>
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new StronglyTypedIdJsonConverterFactory());
        options.Converters.Add(new MoneyJsonConverterFactory());
        options.Converters.Add(new CategoryColorJsonConverterFactory());
        return options;
    });

builder.Services.AddOptions<ScalarOptions>().BindConfiguration("Scalar");
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new StronglyTypedIdJsonConverterFactory());
    options.SerializerOptions.Converters.Add(new MoneyJsonConverterFactory());
    options.SerializerOptions.Converters.Add(new CategoryColorJsonConverterFactory());
});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCustomErrors(app.Environment);

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
else
{
    IdentityModelEventSource.ShowPII = true;
    var prefix = app.Configuration.GetValue<string>("Proxy:Prefix");
    app.MapOpenApi();
    app.MapScalarApiReference(endpointPrefix: prefix ?? "/gateway", options =>
    {
        options.Servers = [];
        options.Authentication = new() {PreferredSecurityScheme = IdentityConstants.BearerScheme};
    });
}

app
    .UseRouting();

app.UseCors()
    .UseAuthentication()
    .UseAuthorization();

app.MapCarter();

if (!builder.Environment.IsEnvironment("SECURITY_TESTS"))
{
    app.UseHangfire(builder.Configuration);
    app.UseHealthChecks();
    app.MigrateDb();
}

await app.RunAsync();
