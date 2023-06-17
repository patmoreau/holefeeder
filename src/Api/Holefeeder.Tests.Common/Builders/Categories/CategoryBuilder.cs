using DrifterApps.Seeds.Testing;
using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Tests.Common.Builders.Categories;

internal class CategoryBuilder : FakerBuilder<Category>
{
    protected override Faker<Category> Faker { get; } = new Faker<Category>()
        .RuleFor(x => x.Id, faker => faker.Random.Guid())
        .RuleFor(x => x.Type, faker => faker.PickRandom<CategoryType>(CategoryType.List))
        .RuleFor(x => x.Name, faker => faker.Lorem.Word() + $" #{faker.IndexFaker}")
        .RuleFor(x => x.Color, faker => faker.Internet.Color())
        .RuleFor(x => x.Favorite, faker => faker.Random.Bool())
        .RuleFor(x => x.System, false)
        .RuleFor(x => x.BudgetAmount, faker => faker.Finance.Amount())
        .RuleFor(x => x.UserId, faker => faker.Random.Guid());

    public static CategoryBuilder GivenACategory() => new();

    public CategoryBuilder WithId(Guid id)
    {
        Faker.RuleFor(f => f.Id, id);
        return this;
    }

    public CategoryBuilder WithNoId()
    {
        Faker.RuleFor(f => f.Id, Guid.Empty);
        return this;
    }

    public CategoryBuilder OfType(CategoryType type)
    {
        Faker.RuleFor(f => f.Type, type);
        return this;
    }

    public CategoryBuilder WithName(string name)
    {
        Faker.RuleFor(f => f.Name, name);
        return this;
    }

    public CategoryBuilder ForUser(Guid userId)
    {
        Faker.RuleFor(f => f.UserId, userId);
        return this;
    }

    public CategoryBuilder ForNoUser()
    {
        Faker.RuleFor(f => f.UserId, Guid.Empty);
        return this;
    }

    public CategoryBuilder IsFavorite()
    {
        Faker.RuleFor(f => f.Favorite, true);
        return this;
    }

    public CategoryBuilder IsNotFavorite()
    {
        Faker.RuleFor(f => f.Favorite, false);
        return this;
    }
}
