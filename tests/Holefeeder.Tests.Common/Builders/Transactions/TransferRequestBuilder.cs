using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Transactions.Commands.Transfer;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class TransferRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateUninitializedFaker()
        .RuleFor(x => x.Date, faker => faker.Date.RecentDateOnly())
        .RuleFor(x => x.Amount, MoneyBuilder.Create().Build())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.FromAccountId, faker => (AccountId)faker.RandomGuid())
        .RuleFor(x => x.ToAccountId, faker => (AccountId)faker.RandomGuid());

    public TransferRequestBuilder FromAccount(Account account)
    {
        Faker.RuleFor(x => x.FromAccountId, account.Id);
        return this;
    }

    public TransferRequestBuilder WithNoDestinationAccount()
    {
        Faker.RuleFor(x => x.ToAccountId, faker => faker.PickRandom(AccountId.Empty, default, null!));
        return this;
    }

    public TransferRequestBuilder WithNoSourceAccount()
    {
        Faker.RuleFor(x => x.FromAccountId, faker => faker.PickRandom(AccountId.Empty, default, null!));
        return this;
    }

    public TransferRequestBuilder ToAccount(Account account)
    {
        Faker.RuleFor(x => x.ToAccountId, account.Id);
        return this;
    }

    public TransferRequestBuilder WithNoDate()
    {
        Faker.RuleFor(x => x.Date, faker => faker.PickRandom(DateOnly.MinValue, default));
        return this;
    }

    public TransferRequestBuilder WithAmount(Money amount)
    {
        Faker.RuleFor(x => x.Amount, amount);
        return this;
    }

    public static TransferRequestBuilder GivenATransfer() => new();

    public static TransferRequestBuilder GivenAnInvalidTransfer()
    {
        var builder = new TransferRequestBuilder();
        builder.Faker.RuleFor(x => x.FromAccountId, AccountId.Empty);
        return builder;
    }
}
