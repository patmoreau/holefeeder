using DrifterApps.Seeds.Testing;
using Holefeeder.Domain.Features.Accounts;
using static Holefeeder.Application.Features.Transactions.Commands.Transfer;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class TransferRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.Date.RecentDateOnly(), faker.Finance.Amount(),
            faker.Lorem.Sentence(),
            faker.Random.Guid(), faker.Random.Guid()))
        .RuleFor(x => x.Date, faker => faker.Date.RecentDateOnly())
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.FromAccountId, faker => faker.Random.Guid())
        .RuleFor(x => x.ToAccountId, faker => faker.Random.Guid());

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
