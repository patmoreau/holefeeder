using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Api.GetTransaction;
using Holefeeder.Service.Transactions.Core.GetTransaction;

namespace Holefeeder.ServiceTests.Transactions.Api.GetTransaction;

public class GetTransactionRequestRawTests
{
    [Fact]
    public void ToGetTransactionRequest_ReturnsTheTransactionForTheUser_WhenIdIsValid()
    {
        var user = AUser();
        var id = AnId();

        var result = GetTransactionRequestRaw.ToGetTransactionRequest(new(id.Value.ToString()), user);

        result.Should().BeSuccessful().And.WithValue(new GetTransactionRequest(user, id));
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("")]
    [InlineData("not-a-guid")]
    public void ToGetTransactionRequest_FailsValidation_WhenIdIsInvalid(string idRaw)
    {
        var result = GetTransactionRequestRaw.ToGetTransactionRequest(new(idRaw), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Should().ContainKey(IdErrors.CodeInvalid);
    }
}
