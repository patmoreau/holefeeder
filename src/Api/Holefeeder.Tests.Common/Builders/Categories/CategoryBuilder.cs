using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Tests.Common.Builders.Categories;

internal class CategoryBuilder : IBuilder<Category>
{
    private Category _entity;

    private CategoryBuilder()
    {
        _entity = new CategoryFactory().Generate();
    }

    public Category Build()
    {
        return _entity;
    }

    public static CategoryBuilder GivenACategory()
    {
        return new();
    }

    public CategoryBuilder OfType(CategoryType type)
    {
        _entity = _entity with {Type = type};
        return this;
    }

    public CategoryBuilder WithName(string name)
    {
        _entity = _entity with {Name = name};
        return this;
    }

    public CategoryBuilder ForUser(Guid userId)
    {
        _entity = _entity with {UserId = userId};
        return this;
    }
}
