using DrifterApps.Seeds.Testing;

using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Transactions.Commands.DeleteTransaction;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class DeleteTransactionRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> FakerRules { get; } = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.RandomGuid()));

    public DeleteTransactionRequestBuilder WithId(Guid id)
    {
        FakerRules.RuleFor(x => x.Id, id);
        return this;
    }

    public DeleteTransactionRequestBuilder WithNoId()
    {
        FakerRules.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static DeleteTransactionRequestBuilder GivenADeleteTransactionRequest() => new();

    public static DeleteTransactionRequestBuilder GivenAnInvalidDeleteTransactionRequest() =>
        new DeleteTransactionRequestBuilder().WithNoId();
}
