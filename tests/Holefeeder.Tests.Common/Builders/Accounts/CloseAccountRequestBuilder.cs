using DrifterApps.Seeds.Testing;

using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Accounts.Commands.CloseAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class CloseAccountRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> FakerRules { get; } = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.RandomGuid()))
        .RuleFor(x => x.Id, faker => faker.RandomGuid());

    public CloseAccountRequestBuilder WithId(Guid id)
    {
        FakerRules.RuleFor(x => x.Id, id);
        return this;
    }

    public CloseAccountRequestBuilder WithNoId()
    {
        FakerRules.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static CloseAccountRequestBuilder GivenACloseAccountRequest() => new();

    public static CloseAccountRequestBuilder GivenAnInvalidCloseAccountRequest() =>
        new CloseAccountRequestBuilder().WithNoId();
}
