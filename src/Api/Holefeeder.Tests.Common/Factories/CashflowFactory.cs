using AutoBogus;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Tests.Common.Factories;

internal sealed class CashflowFactory : AutoFaker<Cashflow>
{
    private const decimal AMOUNT_MAX = 100m;

    public CashflowFactory()
    {
        RuleFor(x => x.EffectiveDate, faker => faker.Date.Past().Date);
        RuleFor(x => x.Amount, faker => faker.Finance.Amount(decimal.One, AMOUNT_MAX));

        CustomInstantiator(faker => Cashflow.Create(
            faker.Date.Past().Date,
            faker.PickRandom(DateIntervalType.List.ToArray()),
            faker.Random.Int(1),
            faker.Random.Int(0),
            faker.Finance.Amount(),
            faker.Random.Words(),
            faker.Random.Guid(),
            faker.Random.Guid(),
            faker.Random.Guid()));
    }
}
