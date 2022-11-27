using AutoBogus;

using Bogus;

using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Tests.Common.Builders.Categories;

internal class CategoryBuilder : IBuilder<Category>, ICollectionBuilder<Category>
{
    private const decimal BUDGET_AMOUNT_MAX = 100m;

    private readonly Faker<Category> _faker = new AutoFaker<Category>()
        .RuleFor(x => x.Name, faker => faker.Lorem.Word())
        .RuleFor(x => x.Type, faker => faker.PickRandom(CategoryType.List.ToArray()))
        .RuleFor(x => x.Color, faker => faker.Internet.Color())
        .RuleFor(x => x.BudgetAmount, faker => faker.Finance.Amount(max: BUDGET_AMOUNT_MAX));


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

    public static CategoryBuilder GivenACategory()
    {
        return new();
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
}
