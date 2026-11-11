using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Api.DeleteTransaction;
using Holefeeder.Service.Transactions.Core.DeleteTransaction;

namespace Holefeeder.ServiceTests.Transactions.Api.DeleteTransaction;

public class DeleteTransactionRequestRawTests
{
    [Fact]
    public void ToDeleteTransactionRequest_ReturnsTheTransactionForTheUser_WhenIdIsValid()
    {
        var user = AUser();
        var id = AnId();

        var result = DeleteTransactionRequestRaw.ToDeleteTransactionRequest(new(id.Value.ToString()), user);

        result.Should().BeSuccessful().And.WithValue(new DeleteTransactionRequest(user, id));
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("")]
    [InlineData("not-a-guid")]
    public void ToDeleteTransactionRequest_FailsValidation_WhenIdIsInvalid(string idRaw)
    {
        var result = DeleteTransactionRequestRaw.ToDeleteTransactionRequest(new(idRaw), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Should().ContainKey(IdErrors.CodeInvalid);
    }
}
