using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.MyData.Core.GetImportStatus;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.MyData.Api.GetImportStatus;

internal sealed record GetImportStatusRequestRaw([FromRoute(Name = "id")] string IdRaw)
{
    public static Result<GetImportStatusRequest> ToGetImportStatusRequest(GetImportStatusRequestRaw raw, User user)
    {
        var id = Id.Create(raw.IdRaw);

        var validation = ResultAggregate.Create();
        if (id.IsFailure)
        {
            validation.AddResult(id.Error);
        }

        return validation.OnSuccess<GetImportStatusRequest>(() => new GetImportStatusRequest(user, id.Value));
    }
}
