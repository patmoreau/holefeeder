using static Holefeeder.Application.Features.Accounts.Commands.OpenAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class OpenAccountRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>()
        .RuleFor(x => x.OpenDate, faker => faker.Date.Past().Date);

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public OpenAccountRequestBuilder WithName(string name)
    {
        _faker.RuleFor(x => x.Name, name);
        return this;
    }

    public OpenAccountRequestBuilder WithNoName()
    {
        _faker.RuleFor(x => x.Name, string.Empty);
        return this;
    }

    public static OpenAccountRequestBuilder GivenAnOpenAccountRequest() => new();

    public static OpenAccountRequestBuilder GivenAnInvalidOpenAccountRequest() =>
        new OpenAccountRequestBuilder().WithNoName();
}
