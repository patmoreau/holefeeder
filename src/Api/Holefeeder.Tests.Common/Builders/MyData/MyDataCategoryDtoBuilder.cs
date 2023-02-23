using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataCategoryDtoBuilder : IBuilder<MyDataCategoryDto>, ICollectionBuilder<MyDataCategoryDto>
{
    private readonly Faker<MyDataCategoryDto> _faker = new AutoFaker<MyDataCategoryDto>()
        .RuleFor(f => f.Type, faker => faker.PickRandom(CategoryType.List.ToArray()));

    public MyDataCategoryDto Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public MyDataCategoryDto[] Build(int count)
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate(count).ToArray();
    }

    public MyDataCategoryDto[] Build(Faker faker) => Build(faker.Random.Int(1, 10));

    public static MyDataCategoryDtoBuilder GivenMyCategoryData() => new();
}
