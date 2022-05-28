using AutoBogus;

using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Tests.Common.Factories;

internal sealed class TransactionFactory : AutoFaker<Transaction>
{
    private const decimal AMOUNT_MAX = 100m;

    public TransactionFactory()
    {
        RuleFor(x => x.Date, faker => faker.Date.Past().Date);
        RuleFor(x => x.Amount, faker => faker.Finance.Amount(min: Decimal.Zero, max: AMOUNT_MAX));
        RuleFor(x => x.CashflowId, _ => null);
        RuleFor(x => x.CashflowDate, _ => null);

        CustomInstantiator(faker => Transaction.Create(faker.Random.Guid(),
            faker.Date.Past().Date,
            faker.Finance.Amount(min: Decimal.Zero, max: AMOUNT_MAX),
            faker.Random.Words(), faker.Random.Guid(), faker.Random.Guid(), faker.Random.Guid()));
    }
}
