using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Tags.Core;
using Holefeeder.Service.Tags.Core.DeleteTag;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Tags.Api.DeleteTag;

internal sealed record DeleteTagRequestRaw([FromBody] DeleteTagBody Body)
{
    public static Result<DeleteTagRequest> ToDeleteTagRequest(DeleteTagRequestRaw raw, User user)
    {
        var tag = TagName.Existing(raw.Body.Tag);

        var validation = ResultAggregate.Create();
        if (tag.IsFailure)
        {
            validation.AddResult(tag.Error);
        }

        return validation.OnSuccess<DeleteTagRequest>(() => new DeleteTagRequest(user, tag.Value));
    }
}
