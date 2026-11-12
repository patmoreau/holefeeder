using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Tags.Core;
using Holefeeder.Service.Tags.Core.ListTags;

namespace Holefeeder.ServiceTests.Tags.TestKit;

internal sealed class TagsInMemory : ITags
{
    private readonly List<(Id UserId, TagSummary Summary)> _summaries = [];
    private ResultError? _failure;

    public Task<Result<IReadOnlyList<TagSummary>>> ListAsync(User user, CancellationToken cancellationToken) =>
        Task.FromResult<Result<IReadOnlyList<TagSummary>>>(
            _failure is not null
                ? _failure
                : _summaries.Where(entry => entry.UserId == user.Id).Select(entry => entry.Summary).ToList());

    public void Add(User user, TagSummary summary) => _summaries.Add((user.Id, summary));

    public void FailWith(ResultError error) => _failure = error;
}
