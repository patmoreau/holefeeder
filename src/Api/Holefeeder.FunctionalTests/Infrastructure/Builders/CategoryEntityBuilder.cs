using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Infrastructure.Factories;
using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.FunctionalTests.Infrastructure.Builders;

internal class CategoryEntityBuilder : IEntityBuilder<CategoryEntity>
{
    private CategoryEntity _entity;

    public static CategoryEntityBuilder GivenACategory() => new();

    private CategoryEntityBuilder() => _entity = new CategoryEntityFactory().Generate();

    public CategoryEntityBuilder OfType(CategoryType type)
    {
        _entity = _entity with {Type = type};
        return this;
    }
    public CategoryEntityBuilder ForUser(Guid userId)
    {
        _entity = _entity with {UserId = userId};
        return this;
    }

    public CategoryEntity Build() => _entity;
}
