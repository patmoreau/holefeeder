using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.StoreItems.TestKit;

internal sealed class StoreItemsInMemory : IStoreItems
{
    private readonly List<(Id UserId, StoreItem Item)> _storeItems = [];
    private ResultError? _failure;

    public Task<Result<StoreItemsPage>> SearchAsync(SearchStoreItemsRequest request, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<StoreItemsPage>>(_failure);
        }

        var matching = _storeItems
            .Where(entry => entry.UserId == request.User.Id)
            .Select(entry => entry.Item)
            .Where(item => MatchesFilter(item, request.FilterBy))
            .ToList();
        var sorted = request.SortBy.Direction == SortByDirection.Descending
            ? matching.OrderByDescending(item => item.Code.Value, StringComparer.Ordinal)
            : matching.OrderBy(item => item.Code.Value, StringComparer.Ordinal);
        var page = sorted.Skip(request.Offset.Value).Take(request.Limit.Value).ToList();

        return Task.FromResult<Result<StoreItemsPage>>(new StoreItemsPage(matching.Count, page));
    }

    public void Add(User user, StoreItem storeItem) => _storeItems.Add((user.Id, storeItem));

    public void FailWith(ResultError error) => _failure = error;

    private static bool MatchesFilter(StoreItem item, SearchStoreItemsRequestFilterBy filterBy) =>
        filterBy switch
        {
            SearchStoreItemsRequestFilterBy.NoFilter => true,
            SearchStoreItemsRequestFilterBy.ByCode { Operator: FilterByOperator.Equal } byCode => item.Code == byCode.Code,
            SearchStoreItemsRequestFilterBy.ByCode { Operator: FilterByOperator.NotEqual } byCode => item.Code != byCode.Code,
            _ => throw new NotSupportedException($"{filterBy} is not supported by store items search.")
        };
}
