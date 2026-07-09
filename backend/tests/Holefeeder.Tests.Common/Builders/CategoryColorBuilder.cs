using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Tests.Common.Builders;

internal class CategoryColorBuilder
{
    private readonly Faker _faker = new();

    private CategoryColorBuilder()
    {
    }

    public CategoryColor Build() => CategoryColor.Create(_faker.Internet.Color()).Value;

    public static CategoryColorBuilder Create() => new CategoryColorBuilder();
}
