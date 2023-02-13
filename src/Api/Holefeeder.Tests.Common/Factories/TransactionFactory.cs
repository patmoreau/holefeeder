using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Tests.Common.Factories;

internal sealed class TransactionFactory : AutoFaker<Transaction>
{
    public TransactionFactory()
    {
        RuleFor(x => x.CashflowId, _ => null);
        RuleFor(x => x.CashflowDate, _ => null);

        CustomInstantiator(faker => Transaction.Create(faker.Random.Guid(),
            faker.Date.Past().Date,
            faker.Finance.Amount(),
            faker.Random.Words(), faker.Random.Guid(), faker.Random.Guid(), faker.Random.Guid()));
    }
}
