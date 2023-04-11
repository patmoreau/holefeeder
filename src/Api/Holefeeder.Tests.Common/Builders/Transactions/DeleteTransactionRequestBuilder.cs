using Holefeeder.Tests.Common.SeedWork;
using static Holefeeder.Application.Features.Transactions.Commands.DeleteTransaction;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class DeleteTransactionRequestBuilder : RootBuilder<Request>
{
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
