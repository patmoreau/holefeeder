using static Holefeeder.Application.Features.Accounts.Commands.FavoriteAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class FavoriteAccountRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>();

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public FavoriteAccountRequestBuilder WithId(Guid id)
    {
        _faker.RuleFor(x => x.Id, id);
        return this;
    }

    public FavoriteAccountRequestBuilder IsFavorite()
    {
        _faker.RuleFor(x => x.IsFavorite, true);
        return this;
    }

    public FavoriteAccountRequestBuilder IsNotFavorite()
    {
        _faker.RuleFor(x => x.IsFavorite, false);
        return this;
    }

    public FavoriteAccountRequestBuilder WithNoId()
    {
        _faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static FavoriteAccountRequestBuilder GivenAFavoriteAccountRequest() => new();

    public static FavoriteAccountRequestBuilder GivenAnInvalidFavoriteAccountRequest() =>
        new FavoriteAccountRequestBuilder().WithNoId();
}
