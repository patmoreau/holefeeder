using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Tests.Common.SeedWork;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataCategoryDtoBuilder : RootBuilder<MyDataCategoryDto>
{
    protected override Faker<MyDataCategoryDto> Faker { get; } = new AutoFaker<MyDataCategoryDto>()
        .RuleFor(f => f.Type, faker => faker.PickRandom(CategoryType.List.ToArray()))
        .RuleFor(f => f.Color, faker => faker.Internet.Color());

    public static MyDataCategoryDtoBuilder GivenMyCategoryData() => new();
}
