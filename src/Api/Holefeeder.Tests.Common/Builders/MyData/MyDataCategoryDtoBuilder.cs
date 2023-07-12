using DrifterApps.Seeds.Testing;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataCategoryDtoBuilder : FakerBuilder<MyDataCategoryDto>
{
    protected override Faker<MyDataCategoryDto> Faker { get; } = new Faker<MyDataCategoryDto>()
        .RuleFor(f => f.Id, faker => faker.Random.Guid())
        .RuleFor(f => f.Type, faker => faker.PickRandom(CategoryType.List.ToArray()))
        .RuleFor(f => f.Name, faker => faker.Lorem.Word())
        .RuleFor(f => f.Color, faker => faker.Internet.Color())
        .RuleFor(f => f.BudgetAmount, faker => faker.Finance.Amount())
        .RuleFor(f => f.Favorite, faker => faker.Random.Bool())
        .RuleFor(f => f.System, faker => faker.Random.Bool());

    public static MyDataCategoryDtoBuilder GivenMyCategoryData() => new();
}
