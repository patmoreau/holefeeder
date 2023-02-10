using Carter;

using Hangfire;

using Holefeeder.Api.ErrorHandling;
using Holefeeder.Api.Extensions;
using Holefeeder.Application.Extensions;
using Holefeeder.Infrastructure.Extensions;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddCarter(configurator: configurator => configurator.WithEmptyValidators())
    .AddSwagger(builder.Environment)
    .AddHealthChecks(builder.Configuration)
    .AddSecurity(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCustomErrors(app.Environment);

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.MapSwagger(builder.Environment)
    .MapHealthChecks()
    .MapCarter();
app.UseHealthChecks("/ready");
app.UseHealthChecks("/health/startup");
app.UseHangfireDashboard();
app.MapHangfireDashboard();

app.UseAuthentication()
    .UseAuthorization();

app.MigrateDb();

await app.RunAsync();
