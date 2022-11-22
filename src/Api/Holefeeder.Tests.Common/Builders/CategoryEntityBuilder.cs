using AutoBogus;

using Bogus;

using Holefeeder.Domain.Features.Categories;
using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.Tests.Common.Builders;

internal class CategoryEntityBuilder : IBuilder<CategoryEntity>, ICollectionBuilder<CategoryEntity>
{
    private const decimal BUDGET_AMOUNT_MAX = 100m;

    private readonly Faker<CategoryEntity> _faker = new AutoFaker<CategoryEntity>()
        .RuleFor(x => x.Name, faker => faker.Lorem.Word())
        .RuleFor(x => x.Type, faker => faker.PickRandom(CategoryType.List.ToArray()))
        .RuleFor(x => x.Color, faker => faker.Internet.Color())
        .RuleFor(x => x.BudgetAmount, faker => faker.Finance.Amount(max: BUDGET_AMOUNT_MAX));

    public static CategoryEntityBuilder GivenACategory() => new();

    public CategoryEntityBuilder OfType(CategoryType type)
    {
        _faker.RuleFor(f => f.Type, type);
        return this;
    }

    public CategoryEntityBuilder WithName(string name)
    {
        _faker.RuleFor(f => f.Name, name);
        return this;
    }

    public CategoryEntityBuilder ForUser(Guid userId)
    {
        _faker.RuleFor(f => f.UserId, userId);
        return this;
    }

    public CategoryEntity Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public CategoryEntity[] Build(int count)
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate(count).ToArray();
    }
}
