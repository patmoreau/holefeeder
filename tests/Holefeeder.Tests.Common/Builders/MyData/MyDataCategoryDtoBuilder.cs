using DrifterApps.Seeds.Testing;

using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Tests.Common.Extensions;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataCategoryDtoBuilder : FakerBuilder<MyDataCategoryDto>
{
    protected override Faker<MyDataCategoryDto> Faker { get; } = CreateFaker<MyDataCategoryDto>()
        .RuleFor(f => f.Id, faker => faker.RandomGuid())
        .RuleFor(f => f.Type, faker => faker.PickRandom(CategoryType.List.ToArray()))
        .RuleFor(f => f.Name, faker => faker.Lorem.Word())
        .RuleFor(f => f.Color, faker => faker.Internet.Color())
        .RuleFor(f => f.BudgetAmount, faker => faker.Finance.Amount())
        .RuleFor(f => f.Favorite, faker => faker.Random.Bool())
        .RuleFor(f => f.System, faker => faker.Random.Bool());

    public static MyDataCategoryDtoBuilder GivenMyCategoryData() => new();
}
