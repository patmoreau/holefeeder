using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Tags.Core;
using Holefeeder.Service.Tags.Core.ListTags;

namespace Holefeeder.ServiceTests.Tags.TestKit;

internal sealed class TagsInMemory : ITags
{
    private readonly List<(Id UserId, TagSummary Summary)> _summaries = [];
    private readonly List<(Id UserId, string Tags)> _tagged = [];
    private ResultError? _failure;

    public Task<Result<IReadOnlyList<TagSummary>>> ListAsync(User user, CancellationToken cancellationToken) =>
        Task.FromResult<Result<IReadOnlyList<TagSummary>>>(
            _failure is not null
                ? _failure
                : _summaries.Where(entry => entry.UserId == user.Id).Select(entry => entry.Summary).ToList());

    public Task<Result<Nothing>> RenameAsync(User user, TagName tag, TagName newTag, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<Nothing>>(_failure);
        }

        for (var index = 0; index < _tagged.Count; index++)
        {
            if (_tagged[index].UserId == user.Id)
            {
                _tagged[index] = _tagged[index] with { Tags = TagText.Rename(_tagged[index].Tags, tag, newTag) };
            }
        }

        return Task.FromResult<Result<Nothing>>(Nothing.Value);
    }

    public void Add(User user, TagSummary summary) => _summaries.Add((user.Id, summary));

    public int Tagged(User user, string tags)
    {
        _tagged.Add((user.Id, tags));
        return _tagged.Count - 1;
    }

    public string TagsOf(int item) => _tagged[item].Tags;

    public void FailWith(ResultError error) => _failure = error;
}
