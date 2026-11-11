using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Api.FavoriteAccount;
using Holefeeder.Service.Accounts.Core.FavoriteAccount;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Accounts.Api.FavoriteAccount;

public class FavoriteAccountRequestRawTests
{
    private const string AccountId = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("True", true)]
    [InlineData(null, false)]
    public void ToFavoriteAccountRequest_ReturnsTheFavoriteFlagForTheUser(string? isFavorite, bool expected)
    {
        var user = AUser();

        var result = FavoriteAccountRequestRaw.ToFavoriteAccountRequest(new(new FavoriteAccountBody(AccountId, isFavorite)), user);

        result.Should().BeSuccessful().And.WithValue(new FavoriteAccountRequest(user, Id.Valid(Guid.Parse(AccountId)), expected));
    }

    [Fact]
    public void ToFavoriteAccountRequest_Fails_WithEveryInvalidField()
    {
        var result = FavoriteAccountRequestRaw.ToFavoriteAccountRequest(new(new FavoriteAccountBody(Guid.Empty.ToString(), "yes")), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys.Should().BeEquivalentTo(
            IdErrors.CodeInvalid,
            FavoriteAccountRequestErrors.CodeIsFavoriteInvalid);
    }

    [Fact]
    public void IsFavoriteInvalid_DescribesTheFlagRule()
    {
        FavoriteAccountRequestErrors.IsFavoriteInvalid.Code.Should().Be("is-favorite-invalid");
        FavoriteAccountRequestErrors.IsFavoriteInvalid.Description.Should().Be("Is favorite must be true or false.");
    }
}
