using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Tags.Core.ListTags;

internal sealed class ListTagsUseCase(ITags tags)
{
    public Task<Result<IReadOnlyList<TagSummary>>> HandleAsync(ListTagsRequest request, CancellationToken cancellationToken) =>
        tags.ListAsync(request.User, cancellationToken);
}
