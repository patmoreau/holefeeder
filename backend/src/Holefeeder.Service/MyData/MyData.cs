using Holefeeder.Service.MyData.Api.ExportData;
using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.MyData.Core.ExportData;
using Holefeeder.Service.MyData.Persistence;

namespace Holefeeder.Service.MyData;

internal static class MyData
{
    public static IServiceCollection AddMyDataServices(this IServiceCollection services) =>
        services
            .AddScoped<IMyDataStore, MyDataInDatabase>()
            .AddScoped<ExportDataUseCase>();

    public static IEndpointRouteBuilder MapMyDataEndpoints(this IEndpointRouteBuilder app) =>
        app.MapExportData();
}
