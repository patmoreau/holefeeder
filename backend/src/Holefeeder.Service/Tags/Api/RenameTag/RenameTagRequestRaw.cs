using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Tags.Core;
using Holefeeder.Service.Tags.Core.RenameTag;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Tags.Api.RenameTag;

internal sealed record RenameTagRequestRaw([FromBody] RenameTagBody Body)
{
    public static Result<RenameTagRequest> ToRenameTagRequest(RenameTagRequestRaw raw, User user)
    {
        var tag = TagName.Existing(raw.Body.Tag);
        var newTag = TagName.Create(raw.Body.NewTag);

        var validation = ResultAggregate.Create();
        if (tag.IsFailure)
        {
            validation.AddResult(tag.Error);
        }
        if (newTag.IsFailure)
        {
            validation.AddResult(newTag.Error);
        }

        return validation.OnSuccess<RenameTagRequest>(() => new RenameTagRequest(user, tag.Value, newTag.Value));
    }
}
