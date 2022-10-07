using Holefeeder.Domain.Features.Categories;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Tests.Common.Factories;

namespace Holefeeder.Tests.Common.Builders;

internal class CategoryEntityBuilder : IBuilder<CategoryEntity>
{
    private CategoryEntity _entity;

    private CategoryEntityBuilder()
    {
        _entity = new CategoryEntityFactory().Generate();
    }

    public CategoryEntity Build()
    {
        return _entity;
    }

    public static CategoryEntityBuilder GivenACategory()
    {
        return new();
    }

    public CategoryEntityBuilder OfType(CategoryType type)
    {
        _entity = _entity with {Type = type};
        return this;
    }

    public CategoryEntityBuilder WithName(string name)
    {
        _entity = _entity with {Name = name};
        return this;
    }

    public CategoryEntityBuilder ForUser(Guid userId)
    {
        _entity = _entity with {UserId = userId};
        return this;
    }
}
