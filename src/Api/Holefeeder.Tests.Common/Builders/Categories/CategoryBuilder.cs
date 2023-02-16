using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Tests.Common.Builders.Categories;

internal class CategoryBuilder : IBuilder<Category>, ICollectionBuilder<Category>
{
    private readonly Faker<Category> _faker = new AutoFaker<Category>()
        .RuleFor(x => x.Name, faker => faker.Lorem.Word() + $" #{faker.IndexFaker}")
        .RuleFor(x => x.Color, faker => faker.Internet.Color());


    public Category Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public Category[] Build(int count)
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate(count).ToArray();
    }

    public Category[] Build(Faker faker) => this.Build(faker.Random.Int(1, 10));

    public static CategoryBuilder GivenACategory() => new();

    public CategoryBuilder WithId(Guid id)
    {
        _faker.RuleFor(f => f.Id, id);
        return this;
    }

    public CategoryBuilder OfType(CategoryType type)
    {
        _faker.RuleFor(f => f.Type, type);
        return this;
    }

    public CategoryBuilder WithName(string name)
    {
        _faker.RuleFor(f => f.Name, name);
        return this;
    }

    public CategoryBuilder ForUser(Guid userId)
    {
        _faker.RuleFor(f => f.UserId, userId);
        return this;
    }

    public CategoryBuilder IsFavorite()
    {
        _faker.RuleFor(f => f.Favorite, true);
        return this;
    }

    public CategoryBuilder IsNotFavorite()
    {
        _faker.RuleFor(f => f.Favorite, false);
        return this;
    }
}
