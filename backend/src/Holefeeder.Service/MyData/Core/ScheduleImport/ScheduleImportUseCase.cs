using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.MyData.Core.ImportData;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.MyData.Core.ScheduleImport;

internal sealed class ScheduleImportUseCase(IImportStatuses statuses, IImportQueue queue)
{
    public Result<Id> Handle(ImportDataRequest request)
    {
        if (request.User.Id == Id.Empty)
        {
            return UserErrors.NotRegistered;
        }

        var importId = Id.New();
        statuses.Save(ImportStatus.Initializing(importId, request.User.Id));

        var enqueued = queue.Enqueue(importId, request);
        return enqueued.IsFailure ? enqueued.Error : importId;
    }
}
