using Holefeeder.Service.MyData.Core.ImportData;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.MyData.Jobs.ImportData;

internal sealed partial class ImportDataJob(ImportDataUseCase useCase, ILogger<ImportDataJob> logger)
{
    public async Task RunAsync(ImportDataPayload payload, CancellationToken cancellationToken)
    {
        var result = await useCase.HandleAsync(Id.Valid(payload.ImportId), payload.ToRequest(), cancellationToken);
        if (result.IsFailure)
        {
            LogImportFailed(logger, payload.ImportId, result.Error.Code);
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Import {ImportId} failed with {Code}")]
    private static partial void LogImportFailed(ILogger logger, Guid importId, string code);
}
