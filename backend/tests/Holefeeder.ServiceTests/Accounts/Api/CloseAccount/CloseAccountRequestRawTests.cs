using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Api.CloseAccount;
using Holefeeder.Service.Accounts.Core.CloseAccount;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Accounts.Api.CloseAccount;

public class CloseAccountRequestRawTests
{
    private const string AccountId = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";

    [Fact]
    public void ToCloseAccountRequest_ReturnsTheAccountToCloseForTheUser()
    {
        var user = AUser();

        var result = CloseAccountRequestRaw.ToCloseAccountRequest(new(new CloseAccountBody(AccountId)), user);

        result.Should().BeSuccessful().And.WithValue(new CloseAccountRequest(user, Id.Valid(Guid.Parse(AccountId))));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("chequing")]
    public void ToCloseAccountRequest_Fails_WhenTheIdIsInvalid(string? id)
    {
        var result = CloseAccountRequestRaw.ToCloseAccountRequest(new(new CloseAccountBody(id)), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys.Should().BeEquivalentTo(IdErrors.CodeInvalid);
    }
}
