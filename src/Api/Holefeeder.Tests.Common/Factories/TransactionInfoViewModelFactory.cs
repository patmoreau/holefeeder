using System.Collections.Immutable;

using AutoBogus;

using Holefeeder.Application.Models;

namespace Holefeeder.Tests.Common.Factories;

internal sealed class TransactionInfoViewModelFactory : AutoFaker<TransactionInfoViewModel>
{
    private const decimal AMOUNT_MAX = 100m;

    public TransactionInfoViewModelFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.Date, faker => faker.Date.Past().Date);
        RuleFor(x => x.Amount, faker => faker.Finance.Amount(Decimal.Zero, AMOUNT_MAX));
        RuleFor(x => x.Description, faker => faker.Random.Words());
        RuleFor(x => x.Tags, faker => ImmutableArray.Create(faker.Random.WordsArray(0, 5)));
    }
}
