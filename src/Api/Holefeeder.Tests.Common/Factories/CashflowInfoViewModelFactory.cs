using System.Collections.Immutable;

using AutoBogus;

using Holefeeder.Application.Models;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Tests.Common.Factories;

internal sealed class CashflowInfoViewModelFactory : AutoFaker<CashflowInfoViewModel>
{
    private const decimal AMOUNT_MAX = 100m;

    public CashflowInfoViewModelFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.EffectiveDate, faker => faker.Date.Past().Date);
        RuleFor(x => x.Amount, faker => faker.Finance.Amount(min: Decimal.Zero, max: AMOUNT_MAX));
        RuleFor(x => x.IntervalType, faker => faker.PickRandom(Enumeration.GetAll<DateIntervalType>()));
        RuleFor(x => x.Frequency, faker => faker.Random.Int());
        RuleFor(x => x.Recurrence, faker => faker.Random.Int());
        RuleFor(x => x.Description, faker => faker.Random.Words());
        RuleFor(x => x.Tags, faker => ImmutableArray.Create(faker.Random.WordsArray(0, 5)));
    }
}
