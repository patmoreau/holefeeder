using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Tests.Common.SeedWork;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataCashflowDtoBuilder : RootBuilder<MyDataCashflowDto>
{
    protected override Faker<MyDataCashflowDto> Faker { get; } = new AutoFaker<MyDataCashflowDto>()
        .RuleFor(f => f.IntervalType, faker => faker.PickRandom(DateIntervalType.List.ToArray()))
        .RuleFor(f => f.Frequency, faker => faker.Random.Int(1))
        .RuleFor(f => f.Recurrence, faker => faker.Random.Int(0));

    public static MyDataCashflowDtoBuilder GivenMyCashflowData() => new();

    public MyDataCashflowDtoBuilder WithAccount(MyDataAccountDto account)
    {
        Faker.RuleFor(f => f.AccountId, account.Id);
        return this;
    }

    public MyDataCashflowDtoBuilder WithCategory(MyDataCategoryDto category)
    {
        Faker.RuleFor(f => f.CategoryId, category.Id);
        return this;
    }
}
