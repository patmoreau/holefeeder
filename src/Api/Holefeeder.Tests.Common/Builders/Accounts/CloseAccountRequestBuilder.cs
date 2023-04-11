using Holefeeder.Tests.Common.SeedWork;
using static Holefeeder.Application.Features.Accounts.Commands.CloseAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class CloseAccountRequestBuilder : RootBuilder<Request>
{
    public CloseAccountRequestBuilder WithId(Guid id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public CloseAccountRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static CloseAccountRequestBuilder GivenACloseAccountRequest() => new();

    public static CloseAccountRequestBuilder GivenAnInvalidCloseAccountRequest() =>
        new CloseAccountRequestBuilder().WithNoId();
}
