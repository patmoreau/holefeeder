using DrifterApps.Seeds.Testing;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataCategoryDtoBuilder : FakerBuilder<MyDataCategoryDto>
{
    protected override Faker<MyDataCategoryDto> Faker { get; } = new Faker<MyDataCategoryDto>()
        .RuleFor(f => f.Type, faker => faker.PickRandom(CategoryType.List.ToArray()))
        .RuleFor(f => f.Color, faker => faker.Internet.Color());

    public static MyDataCategoryDtoBuilder GivenMyCategoryData() => new();
}
