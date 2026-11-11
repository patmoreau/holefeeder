using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Api.GetAccountTransactions;
using Holefeeder.Service.Accounts.Core.GetAccountTransactions;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Core.Search;

namespace Holefeeder.ServiceTests.Accounts.Api.GetAccountTransactions;

public class GetAccountTransactionsRequestRawTests
{
    [Fact]
    public void ToGetAccountTransactionsRequest_UsesTheDefaultPage_WhenOffsetAndLimitAreMissing()
    {
        var user = AUser();
        var id = AnId();

        var result = GetAccountTransactionsRequestRaw.ToGetAccountTransactionsRequest(new(id.Value.ToString(), null, null), user);

        result.Should().BeSuccessful().And.WithValue(new GetAccountTransactionsRequest(user, id, Offset.Default, Limit.Default));
    }

    [Fact]
    public void ToGetAccountTransactionsRequest_ReadsTheRequestedPage()
    {
        var user = AUser();
        var id = AnId();

        var result = GetAccountTransactionsRequestRaw.ToGetAccountTransactionsRequest(new(id.Value.ToString(), "20", "10"), user);

        result.Should().BeSuccessful().And.WithValue(new GetAccountTransactionsRequest(user, id, Offset.Valid(20), Limit.Valid(10)));
    }

    [Fact]
    public void ToGetAccountTransactionsRequest_ReportsEveryInvalidValue()
    {
        var result = GetAccountTransactionsRequestRaw.ToGetAccountTransactionsRequest(new(Guid.Empty.ToString(), "-1", "101"), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys
            .Should().BeEquivalentTo(IdErrors.CodeInvalid, OffsetErrors.CodeInvalid, LimitErrors.CodeInvalid);
    }
}
