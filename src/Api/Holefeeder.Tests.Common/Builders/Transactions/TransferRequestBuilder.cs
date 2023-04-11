using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Tests.Common.SeedWork;
using static Holefeeder.Application.Features.Transactions.Commands.Transfer;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class TransferRequestBuilder : RootBuilder<Request>
{
    public TransferRequestBuilder FromAccount(Account account)
    {
        Faker.RuleFor(x => x.FromAccountId, account.Id);
        return this;
    }

    public TransferRequestBuilder ToAccount(Account account)
    {
        Faker.RuleFor(x => x.ToAccountId, account.Id);
        return this;
    }

    public static TransferRequestBuilder GivenATransfer() => new();

    public static TransferRequestBuilder GivenAnInvalidTransfer()
    {
        TransferRequestBuilder builder = new TransferRequestBuilder();
        builder.Faker.RuleFor(x => x.FromAccountId, Guid.Empty);
        return builder;
    }
}
