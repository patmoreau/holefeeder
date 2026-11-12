using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.MyData.Core.GetImportStatus;

internal sealed class GetImportStatusUseCase(IImportStatuses statuses)
{
    public Result<ImportStatus> Handle(GetImportStatusRequest request) => statuses.Find(request.ImportId, request.User.Id);
}
