using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Accounts.Commands.FavoriteAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class FavoriteAccountRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateUninitializedFaker<Request>()
        .RuleFor(x => x.Id, faker => (AccountId)faker.RandomGuid())
        .RuleFor(x => x.IsFavorite, faker => faker.Random.Bool());

    public FavoriteAccountRequestBuilder WithId(AccountId id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public FavoriteAccountRequestBuilder IsFavorite()
    {
        Faker.RuleFor(x => x.IsFavorite, true);
        return this;
    }

    public FavoriteAccountRequestBuilder IsNotFavorite()
    {
        Faker.RuleFor(x => x.IsFavorite, false);
        return this;
    }

    public FavoriteAccountRequestBuilder WithMissingId()
    {
        Faker.RuleFor(x => x.Id, faker => faker.PickRandom(AccountId.Empty, null!));
        return this;
    }

    public static FavoriteAccountRequestBuilder GivenAFavoriteAccountRequest() => new();

    public static FavoriteAccountRequestBuilder GivenAnInvalidFavoriteAccountRequest() =>
        new FavoriteAccountRequestBuilder().WithMissingId();
}
