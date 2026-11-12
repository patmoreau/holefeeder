using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Tags.Core.ListTags;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Tags.Core;

internal interface ITags
{
    Task<Result<IReadOnlyList<TagSummary>>> ListAsync(User user, CancellationToken cancellationToken);
}
