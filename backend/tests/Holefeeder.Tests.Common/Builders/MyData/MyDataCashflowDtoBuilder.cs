using DrifterApps.Seeds.Testing;

using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Tests.Common.Extensions;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataCashflowDtoBuilder : FakerBuilder<MyDataCashflowDto>
{
    protected override Faker<MyDataCashflowDto> Faker { get; } = CreateFaker<MyDataCashflowDto>()
        .RuleFor(f => f.Id, faker => faker.RandomGuid())
        .RuleFor(f => f.EffectiveDate, faker => faker.Date.RecentDateOnly())
        .RuleFor(f => f.Amount, faker => faker.Finance.Amount())
        .RuleFor(f => f.IntervalType, faker => faker.PickRandom<DateIntervalType>(DateIntervalType.List))
        .RuleFor(f => f.Frequency, faker => faker.Random.Int(1))
        .RuleFor(f => f.Recurrence, faker => faker.Random.Int(0))
        .RuleFor(f => f.Description, faker => faker.Lorem.Sentence())
        .RuleFor(f => f.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray())
        .RuleFor(f => f.CategoryId, faker => faker.RandomGuid())
        .RuleFor(f => f.AccountId, faker => faker.RandomGuid())
        .RuleFor(f => f.Inactive, faker => faker.Random.Bool());

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
