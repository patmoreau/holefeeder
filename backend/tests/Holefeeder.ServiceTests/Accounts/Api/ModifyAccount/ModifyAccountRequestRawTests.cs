using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Api.ModifyAccount;
using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.ModifyAccount;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Accounts.Api.ModifyAccount;

public class ModifyAccountRequestRawTests
{
    private const string AccountId = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";

    private static ModifyAccountBody ABody(
        string? id = AccountId, string? name = "Amex Gold", string? openBalance = "99.50", string? description = "Travel card") =>
        new(id, name, openBalance, description);

    [Fact]
    public void ToModifyAccountRequest_ReturnsTheChangesForTheUser()
    {
        var user = AUser();

        var result = ModifyAccountRequestRaw.ToModifyAccountRequest(new(ABody()), user);

        result.Should().BeSuccessful().And.WithValue(new ModifyAccountRequest(
            user, Id.Valid(Guid.Parse(AccountId)), AccountName.Valid("Amex Gold"), Money.Valid(99.50m), "Travel card"));
    }

    [Fact]
    public void ToModifyAccountRequest_LeavesMissingOpenBalanceAndDescriptionUnchanged()
    {
        var result = ModifyAccountRequestRaw.ToModifyAccountRequest(new(ABody(openBalance: null, description: null)), AUser());

        result.Should().BeSuccessful();
        result.Value.OpenBalance.Should().BeNull();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public void ToModifyAccountRequest_Fails_WithEveryInvalidField()
    {
        var result = ModifyAccountRequestRaw.ToModifyAccountRequest(
            new(new ModifyAccountBody(Guid.Empty.ToString(), "", "12.345", null)), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys.Should().BeEquivalentTo(
            IdErrors.CodeInvalid,
            AccountNameErrors.CodeInvalid,
            MoneyErrors.CodeInvalid);
    }
}
