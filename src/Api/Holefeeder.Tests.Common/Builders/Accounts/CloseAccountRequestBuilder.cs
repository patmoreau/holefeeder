using static Holefeeder.Application.Features.Accounts.Commands.CloseAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class CloseAccountRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>();

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public CloseAccountRequestBuilder WithId(Guid id)
    {
        _faker.RuleFor(x => x.Id, id);
        return this;
    }

    public CloseAccountRequestBuilder WithNoId()
    {
        _faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static CloseAccountRequestBuilder GivenACloseAccountRequest() => new();

    public static CloseAccountRequestBuilder GivenAnInvalidCloseAccountRequest() =>
        new CloseAccountRequestBuilder().WithNoId();
}
