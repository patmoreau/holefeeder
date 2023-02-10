using Holefeeder.Domain.Features.Accounts;

using static Holefeeder.Application.Features.Transactions.Commands.Transfer;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class TransferRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>();

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public TransferRequestBuilder FromAccount(Account account)
    {
        _faker.RuleFor(x => x.FromAccountId, account.Id);
        return this;
    }

    public TransferRequestBuilder ToAccount(Account account)
    {
        _faker.RuleFor(x => x.ToAccountId, account.Id);
        return this;
    }

    public static TransferRequestBuilder GivenATransfer() => new();

    public static TransferRequestBuilder GivenAnInvalidTransfer()
    {
        var builder = new TransferRequestBuilder();
        builder._faker.RuleFor(x => x.FromAccountId, Guid.Empty);
        return builder;
    }
}
