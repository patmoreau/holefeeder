using System.Diagnostics.CodeAnalysis;

using Serilog;

namespace Holefeeder.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class WebApplicationBuilderExtensions
{
    public static void AddSerilog(this ConfigureHostBuilder host)
    {
        host.UseSerilog((context, _, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
    }
}
