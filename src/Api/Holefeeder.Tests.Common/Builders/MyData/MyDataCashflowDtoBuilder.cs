using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Enumerations;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataCashflowDtoBuilder : IBuilder<MyDataCashflowDto>, ICollectionBuilder<MyDataCashflowDto>
{
    private readonly Faker<MyDataCashflowDto> _faker = new AutoFaker<MyDataCashflowDto>()
        .RuleFor(f => f.IntervalType, faker => faker.PickRandom(DateIntervalType.List.ToArray()))
        .RuleFor(f => f.Frequency, faker => faker.Random.Int(1))
        .RuleFor(f => f.Recurrence, faker => faker.Random.Int(0));

    public MyDataCashflowDto Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public MyDataCashflowDto[] Build(int count)
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate(count).ToArray();
    }

    public MyDataCashflowDto[] Build(Faker faker) => Build(faker.Random.Int(1, 10));

    public static MyDataCashflowDtoBuilder GivenMyCashflowData() => new();

    public MyDataCashflowDtoBuilder WithAccount(MyDataAccountDto account)
    {
        _faker.RuleFor(f => f.AccountId, account.Id);
        return this;
    }

    public MyDataCashflowDtoBuilder WithCategory(MyDataCategoryDto category)
    {
        _faker.RuleFor(f => f.CategoryId, category.Id);
        return this;
    }
}
