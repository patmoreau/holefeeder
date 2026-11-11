using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Api.GetAccountDetail;
using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Accounts.Api.GetAccountDetail;

public class GetAccountDetailRequestRawTests
{
    private static readonly DateOnly Today = new(2026, 11, 10);

    [Fact]
    public void ToGetAccountDetailRequest_ReturnsTheAccountForTheUser_WhenIdIsValid()
    {
        var user = AUser();
        var id = AnId();

        var result = GetAccountDetailRequestRaw.ToGetAccountDetailRequest(new(id.Value.ToString()), user, Today);

        result.Should().BeSuccessful().And.WithValue(new GetAccountDetailRequest(user, id, Today));
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("")]
    [InlineData("not-a-guid")]
    public void ToGetAccountDetailRequest_FailsValidation_WhenIdIsInvalid(string idRaw)
    {
        var result = GetAccountDetailRequestRaw.ToGetAccountDetailRequest(new(idRaw), AUser(), Today);

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Should().ContainKey(IdErrors.CodeInvalid);
    }
}
