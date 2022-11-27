using AutoBogus;

using Bogus;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.Tests.Common.Builders;

internal class CashflowEntityBuilder : IBuilder<CashflowEntity>, ICollectionBuilder<CashflowEntity>
{
    private const decimal AMOUNT_MAX = 100m;

    private readonly Faker<CashflowEntity> _faker = new AutoFaker<CashflowEntity>()
        .RuleFor(x => x.EffectiveDate, faker => faker.Date.Past().Date)
        .RuleFor(x => x.IntervalType, faker => faker.PickRandom(DateIntervalType.List.ToArray()))
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount(min: decimal.Zero, max: AMOUNT_MAX))
        .RuleFor(x => x.Frequency, faker => faker.Random.Int(min: 1))
        .RuleFor(x => x.Recurrence, faker => faker.Random.Int(min: 0))
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.Tags, faker => string.Join(',', faker.Random.WordsArray(0, 5)));

    public static CashflowEntityBuilder GivenACashflow() => new();

    public CashflowEntityBuilder OfAmount(decimal amount)
    {
        _faker.RuleFor(f => f.Amount, amount);
        return this;
    }

    public CashflowEntityBuilder ForAccount(AccountEntity entity)
    {
        _faker.RuleFor(f => f.AccountId, entity.Id)
            .RuleFor(f => f.UserId, entity.UserId);
        return this;
    }

    public CashflowEntityBuilder ForCategory(Category entity)
    {
        _faker.RuleFor(f => f.CategoryId, entity.Id);
        return this;
    }

    public CashflowEntityBuilder ForUser(Guid userId)
    {
        _faker.RuleFor(f => f.UserId, userId);
        return this;
    }

    public CashflowEntity Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public CashflowEntity[] Build(int count)
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate(count).ToArray();
    }
}
