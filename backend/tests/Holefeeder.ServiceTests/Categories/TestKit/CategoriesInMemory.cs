using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.Categories.TestKit;

internal sealed class CategoriesInMemory : ICategories
{
    private readonly List<(Id UserId, Category Category)> _categories = [];
    private ResultError? _failure;

    public Task<Result<IReadOnlyList<Category>>> ListAsync(User user, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<IReadOnlyList<Category>>>(_failure);
        }

        var categories = _categories
            .Where(entry => entry.UserId == user.Id)
            .Select(entry => entry.Category)
            .OrderBy(category => category.Type)
            .ThenByDescending(category => category.Favorite)
            .ThenBy(category => category.Name.Value, StringComparer.Ordinal)
            .ToList();

        return Task.FromResult<Result<IReadOnlyList<Category>>>(categories);
    }

    public void Add(User user, Category category) => _categories.Add((user.Id, category));

    public void FailWith(ResultError error) => _failure = error;
}
