using Carter;

using DrifterApps.Seeds.Infrastructure;

using Holefeeder.Api.ErrorHandling;
using Holefeeder.Api.Extensions;
using Holefeeder.Application.Extensions;
using Holefeeder.Infrastructure.Extensions;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddCors(builder.Configuration)
    .AddSecurity(builder.Configuration)
    .AddCarter(configurator: configurator => configurator.WithEmptyValidators())
    .AddSwagger(builder.Environment, builder.Configuration)
    .AddHealthChecks(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddHangfireRequestScheduler();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCustomErrors(app.Environment);

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseSwagger(builder.Environment, builder.Configuration)
    .UseHealthChecks()
    .UseHangfire()
    .MapCarter();

app.UseAuthentication()
    .UseAuthorization();

app.MigrateDb();

await app.RunAsync();
