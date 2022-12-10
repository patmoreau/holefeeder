using System.Diagnostics.CodeAnalysis;

using Carter;

using Hangfire;

using Holefeeder.Api.ErrorHandling;
using Holefeeder.Api.Extensions;
using Holefeeder.Application.Extensions;
using Holefeeder.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCarter(configurator: configurator => configurator.WithEmptyValidators())
    .AddSwagger(builder.Environment)
    .AddHealthChecks(builder.Configuration)
    .AddSecurity(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

builder.Host.AddSerilog();

var app = builder.Build();

app.UseCustomErrors(app.Environment);

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseSerilog()
    .MapSwagger(builder.Environment)
    .MapHealthChecks()
    .MapCore()
    .MapCarter();

app.UseHangfireDashboard();
app.MapHangfireDashboard();

app.UseAuthentication()
    .UseAuthorization()
    .UseHttpsRedirection();

app.MigrateDb();

await app.RunAsync();

#pragma warning disable CA1050
namespace Holefeeder.Api
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
    }
}
#pragma warning restore CA1050
