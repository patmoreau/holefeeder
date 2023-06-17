using DrifterApps.Seeds.Testing;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Enumerations;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataCashflowDtoBuilder : FakerBuilder<MyDataCashflowDto>
{
    protected override Faker<MyDataCashflowDto> Faker { get; } = new Faker<MyDataCashflowDto>()
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
