using AutoBogus;

using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.FunctionalTests.Infrastructure.Factories;

internal sealed class TransactionEntityFactory : AutoFaker<TransactionEntity>
{
    private const decimal AMOUNT_MAX = 100m;

    public TransactionEntityFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.Date, faker => faker.Date.Past().Date);
        RuleFor(x => x.Amount, faker => faker.Finance.Amount(min: Decimal.Zero, max: AMOUNT_MAX));
        RuleFor(x => x.Description, faker => faker.Random.Words());
        RuleFor(x => x.AccountId, faker => faker.Random.Guid());
        RuleFor(x => x.CategoryId, faker => faker.Random.Guid());
        RuleFor(x => x.CashflowId, _ => null);
        RuleFor(x => x.CashflowDate, _ => null);
        RuleFor(x => x.Tags, faker => string.Join(';', faker.Random.WordsArray(0,5)));
        RuleFor(x => x.UserId, faker => faker.Random.Guid());
    }
}
