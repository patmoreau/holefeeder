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

using ServiceCollectionExtensions = Holefeeder.Api.Extensions.ServiceCollectionExtensions;

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
    })
    .AddHealthChecks(builder.Configuration)
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

// builder.Services.AddOptions<ScalarOptions>().BindConfiguration("Scalar");
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
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Servers = [];
        options.Authentication = new() {PreferredSecurityScheme = IdentityConstants.BearerScheme};
    });
}

app.UseHealthChecks()
    .UseHangfire(builder.Configuration)
    .UseRouting();

app.UseCors(ServiceCollectionExtensions.AllowBlazorClientPolicy)
    .UseAuthentication()
    .UseAuthorization();

app.MapCarter();

app.MigrateDb();

await app.RunAsync();
