using static Holefeeder.Application.Features.Transactions.Commands.DeleteTransaction;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class DeleteTransactionRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>();

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public DeleteTransactionRequestBuilder WithId(Guid id)
    {
        _faker.RuleFor(x => x.Id, id);
        return this;
    }

    public DeleteTransactionRequestBuilder WithNoId()
    {
        _faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static DeleteTransactionRequestBuilder GivenADeleteTransactionRequest() => new();

    public static DeleteTransactionRequestBuilder GivenAnInvalidDeleteTransactionRequest() =>
        new DeleteTransactionRequestBuilder().WithNoId();
}
