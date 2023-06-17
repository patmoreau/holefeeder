using DrifterApps.Seeds.Testing;
using static Holefeeder.Application.Features.Transactions.Commands.DeleteTransaction;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class DeleteTransactionRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.Random.Guid()));

    public DeleteTransactionRequestBuilder WithId(Guid id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public DeleteTransactionRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static DeleteTransactionRequestBuilder GivenADeleteTransactionRequest() => new();

    public static DeleteTransactionRequestBuilder GivenAnInvalidDeleteTransactionRequest() =>
        new DeleteTransactionRequestBuilder().WithNoId();
}
