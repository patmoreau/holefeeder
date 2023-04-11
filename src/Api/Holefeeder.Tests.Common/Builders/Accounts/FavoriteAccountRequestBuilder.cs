using Holefeeder.Tests.Common.SeedWork;
using static Holefeeder.Application.Features.Accounts.Commands.FavoriteAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class FavoriteAccountRequestBuilder : RootBuilder<Request>
{
    public FavoriteAccountRequestBuilder WithId(Guid id)
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

    public FavoriteAccountRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static FavoriteAccountRequestBuilder GivenAFavoriteAccountRequest() => new();

    public static FavoriteAccountRequestBuilder GivenAnInvalidFavoriteAccountRequest() =>
        new FavoriteAccountRequestBuilder().WithNoId();
}
