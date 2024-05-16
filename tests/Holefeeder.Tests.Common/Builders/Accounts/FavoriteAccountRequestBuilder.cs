using DrifterApps.Seeds.Testing;

using static Holefeeder.Application.Features.Accounts.Commands.FavoriteAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class FavoriteAccountRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> FakerRules { get; } = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.Random.Guid(), faker.Random.Bool()))
        .RuleFor(x => x.Id, faker => faker.Random.Guid())
        .RuleFor(x => x.IsFavorite, faker => faker.Random.Bool());

    public FavoriteAccountRequestBuilder WithId(Guid id)
    {
        FakerRules.RuleFor(x => x.Id, id);
        return this;
    }

    public FavoriteAccountRequestBuilder IsFavorite()
    {
        FakerRules.RuleFor(x => x.IsFavorite, true);
        return this;
    }

    public FavoriteAccountRequestBuilder IsNotFavorite()
    {
        FakerRules.RuleFor(x => x.IsFavorite, false);
        return this;
    }

    public FavoriteAccountRequestBuilder WithNoId()
    {
        FakerRules.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static FavoriteAccountRequestBuilder GivenAFavoriteAccountRequest() => new();

    public static FavoriteAccountRequestBuilder GivenAnInvalidFavoriteAccountRequest() =>
        new FavoriteAccountRequestBuilder().WithNoId();
}
