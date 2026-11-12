using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Tags.Core.DeleteTag;

internal sealed class DeleteTagUseCase(ITags tags)
{
    public Task<Result<Nothing>> HandleAsync(DeleteTagRequest request, CancellationToken cancellationToken) =>
        tags.RemoveAsync(request.User, request.Tag, cancellationToken);
}
