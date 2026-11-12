using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.Categories.TestKit;

internal sealed class CategoriesInMemory : ICategories
{
    private readonly List<(Id UserId, Category Category, bool Inactive)> _categories = [];
    private ResultError? _failure;

    public Task<Result<IReadOnlyList<Category>>> ListAsync(User user, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<IReadOnlyList<Category>>>(_failure);
        }

        var categories = _categories
            .Where(entry => entry.UserId == user.Id && !entry.Inactive)
            .Select(entry => entry.Category)
            .OrderBy(category => category.Type)
            .ThenByDescending(category => category.Favorite)
            .ThenBy(category => category.Name.Value, StringComparer.Ordinal)
            .ToList();

        return Task.FromResult<Result<IReadOnlyList<Category>>>(categories);
    }

    public Task<Result<Nothing>> SaveAsync(User user, Category category, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<Nothing>>(_failure);
        }

        var index = _categories.FindIndex(entry => entry.Category.Id == category.Id);
        if (index < 0)
        {
            _categories.Add((user.Id, category, false));
            return Task.FromResult<Result<Nothing>>(Nothing.Value);
        }

        if (_categories[index].UserId != user.Id)
        {
            return Task.FromResult<Result<Nothing>>(CategoryErrors.NotFound);
        }

        _categories[index] = (user.Id, category, _categories[index].Inactive);
        return Task.FromResult<Result<Nothing>>(Nothing.Value);
    }

    public Task<Result<Category>> GetAsync(User user, Id id, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<Category>>(_failure);
        }

        var category = _categories
            .Where(entry => entry.UserId == user.Id && !entry.Inactive && entry.Category.Id == id)
            .Select(entry => entry.Category)
            .SingleOrDefault();

        return Task.FromResult<Result<Category>>(category is null ? CategoryErrors.NotFound : category);
    }

    public Task<Result<bool>> IsNameUsedAsync(User user, CategoryName name, Id exceptId, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<bool>>(_failure);
        }

        return Task.FromResult<Result<bool>>(_categories.Exists(entry =>
            entry.UserId == user.Id && entry.Category.Id != exceptId &&
            string.Equals(entry.Category.Name.Value, name.Value, StringComparison.OrdinalIgnoreCase)));
    }

    public void Add(User user, Category category, bool inactive = false) => _categories.Add((user.Id, category, inactive));

    public void FailWith(ResultError error) => _failure = error;
}
