using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Categories.Core.DeleteCategory;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Categories.Api.DeleteCategory;

internal sealed record DeleteCategoryRequestRaw([FromRoute(Name = "id")] string IdRaw)
{
    public static Result<DeleteCategoryRequest> ToDeleteCategoryRequest(DeleteCategoryRequestRaw raw, User user)
    {
        var id = Id.Create(raw.IdRaw);

        var validation = ResultAggregate.Create();
        if (id.IsFailure)
        {
            validation.AddResult(id.Error);
        }

        return validation.OnSuccess<DeleteCategoryRequest>(() => new DeleteCategoryRequest(user, id.Value));
    }
}
