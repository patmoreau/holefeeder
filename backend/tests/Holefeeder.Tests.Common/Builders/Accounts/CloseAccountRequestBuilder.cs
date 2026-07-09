using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Accounts;

using static Holefeeder.Application.Features.Accounts.Commands.CloseAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class CloseAccountRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateUninitializedFaker<Request>()
            .RuleFor(x => x.Id, faker => (AccountId)faker.Random.Guid());

    public CloseAccountRequestBuilder WithId(AccountId id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public CloseAccountRequestBuilder WithMissingId()
    {
        Faker.RuleFor(x => x.Id, faker => faker.PickRandom(AccountId.Empty, null!));
        return this;
    }

    public static CloseAccountRequestBuilder GivenACloseAccountRequest() => new();

    public static CloseAccountRequestBuilder GivenAnInvalidCloseAccountRequest() =>
        new CloseAccountRequestBuilder().WithMissingId();
}
