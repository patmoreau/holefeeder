using AutoBogus;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.SeedWork;
using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.Tests.Common.Factories;

internal sealed class CashflowEntityFactory : AutoFaker<CashflowEntity>
{
    private const decimal AMOUNT_MAX = 100m;

    public CashflowEntityFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.EffectiveDate, faker => faker.Date.Past().Date);
        RuleFor(x => x.IntervalType, faker => faker.PickRandom(DateIntervalType.List.ToArray()));
        RuleFor(x => x.Amount, faker => faker.Finance.Amount(min: decimal.Zero, max: AMOUNT_MAX));
        RuleFor(x => x.Frequency, faker => faker.Random.Int(min: 1));
        RuleFor(x => x.Recurrence, faker => faker.Random.Int(min: 0));
        RuleFor(x => x.Description, faker => faker.Random.Words());
        RuleFor(x => x.AccountId, faker => faker.Random.Guid());
        RuleFor(x => x.CategoryId, faker => faker.Random.Guid());
        RuleFor(x => x.Tags, faker => string.Join(',', faker.Random.WordsArray(0,5)));
        RuleFor(x => x.UserId, faker => faker.Random.Guid());
    }
}
