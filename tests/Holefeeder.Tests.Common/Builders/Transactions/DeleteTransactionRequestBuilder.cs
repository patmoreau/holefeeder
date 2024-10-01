using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Transactions;

using static Holefeeder.Application.Features.Transactions.Commands.DeleteTransaction;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class DeleteTransactionRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateUninitializedFaker()
        .RuleFor(x => x.Id, faker => (TransactionId)faker.Random.Guid());

    public DeleteTransactionRequestBuilder WithId(TransactionId id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public DeleteTransactionRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, TransactionId.Empty);
        return this;
    }

    public static DeleteTransactionRequestBuilder GivenADeleteTransactionRequest() => new();

    public static DeleteTransactionRequestBuilder GivenAnInvalidDeleteTransactionRequest() =>
        new DeleteTransactionRequestBuilder().WithNoId();
}
