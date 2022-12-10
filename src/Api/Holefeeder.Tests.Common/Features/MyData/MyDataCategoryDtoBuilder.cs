using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Tests.Common.Builders;

namespace Holefeeder.Tests.Common.Features.MyData;

internal sealed class MyDataCategoryDtoBuilder : IBuilder<MyDataCategoryDto>, ICollectionBuilder<MyDataCategoryDto>
{
    private readonly Faker<MyDataCategoryDto> _faker = new AutoFaker<MyDataCategoryDto>()
        .RuleFor(f => f.Type, faker => faker.PickRandom(CategoryType.List.ToArray()));

    public static MyDataCategoryDtoBuilder GivenMyCategoryData() => new();

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
}
