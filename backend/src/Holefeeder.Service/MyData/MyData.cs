using Holefeeder.Service.MyData.Api.ExportData;
using Holefeeder.Service.MyData.Api.GetImportStatus;
using Holefeeder.Service.MyData.Api.ImportData;
using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.MyData.Core.ExportData;
using Holefeeder.Service.MyData.Core.GetImportStatus;
using Holefeeder.Service.MyData.Core.ImportData;
using Holefeeder.Service.MyData.Core.ScheduleImport;
using Holefeeder.Service.MyData.Jobs.ImportData;
using Holefeeder.Service.MyData.Persistence;

namespace Holefeeder.Service.MyData;

internal static class MyData
{
    public static IServiceCollection AddMyDataServices(this IServiceCollection services) =>
        services
            .AddMemoryCache()
            .AddScoped<IMyDataStore, MyDataInDatabase>()
            .AddScoped<IImportStatuses, ImportStatusesInMemoryCache>()
            .AddScoped<IImportQueue, ImportQueueWithHangfire>()
            .AddScoped<ExportDataUseCase>()
            .AddScoped<ScheduleImportUseCase>()
            .AddScoped<ImportDataUseCase>()
            .AddScoped<GetImportStatusUseCase>()
            .AddScoped<ImportDataJob>();

    public static IEndpointRouteBuilder MapMyDataEndpoints(this IEndpointRouteBuilder app) =>
        app.MapExportData()
            .MapImportData()
            .MapGetImportStatus();
}
