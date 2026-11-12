using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Tags.Core.RenameTag;

internal sealed class RenameTagUseCase(ITags tags)
{
    public async Task<Result<Nothing>> HandleAsync(RenameTagRequest request, CancellationToken cancellationToken) =>
        request.Tag == request.NewTag
            ? Nothing.Value
            : await tags.RenameAsync(request.User, request.Tag, request.NewTag, cancellationToken);
}
