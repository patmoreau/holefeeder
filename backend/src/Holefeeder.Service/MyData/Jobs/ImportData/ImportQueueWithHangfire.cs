using DrifterApps.Seeds.FluentResult;

using Hangfire;

using Holefeeder.Service.MyData.Core.ImportData;
using Holefeeder.Service.MyData.Core.ScheduleImport;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.MyData.Jobs.ImportData;

internal sealed partial class ImportQueueWithHangfire(IBackgroundJobClient jobs, ILogger<ImportQueueWithHangfire> logger) : IImportQueue
{
    public Result<Nothing> Enqueue(Id importId, ImportDataRequest request)
    {
        var payload = ImportDataPayload.From(importId, request);
        try
        {
            jobs.Enqueue<ImportDataJob>(job => job.RunAsync(payload, CancellationToken.None));
            return Nothing.Value;
        }
        catch (BackgroundJobClientException exception)
        {
            LogEnqueueFailed(logger, exception);
            return ImportQueueErrors.EnqueueFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "My data import could not be queued")]
    private static partial void LogEnqueueFailed(ILogger logger, Exception exception);
}

internal static class ImportQueueErrors
{
    public const string CodeEnqueueFailed = "my-data-import-enqueue-failed";

    public static UnexpectedError EnqueueFailed => new(CodeEnqueueFailed, "The import could not be queued.");
}
