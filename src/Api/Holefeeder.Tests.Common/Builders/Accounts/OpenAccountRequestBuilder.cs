using DrifterApps.Seeds.Testing;
using static Holefeeder.Application.Features.Accounts.Commands.OpenAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class OpenAccountRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = new Faker<Request>()
        .RuleFor(x => x.OpenDate, faker => faker.Date.Past().Date);

    public OpenAccountRequestBuilder WithName(string name)
    {
        Faker.RuleFor(x => x.Name, name);
        return this;
    }

    public OpenAccountRequestBuilder WithNoName()
    {
        Faker.RuleFor(x => x.Name, string.Empty);
        return this;
    }

    public static OpenAccountRequestBuilder GivenAnOpenAccountRequest() => new();

    public static OpenAccountRequestBuilder GivenAnInvalidOpenAccountRequest() =>
        new OpenAccountRequestBuilder().WithNoName();
}
