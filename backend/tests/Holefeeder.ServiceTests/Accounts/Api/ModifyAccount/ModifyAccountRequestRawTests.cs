using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Api;
using Holefeeder.Service.Accounts.Api.ModifyAccount;
using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.ModifyAccount;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Accounts.Api.ModifyAccount;

public class ModifyAccountRequestRawTests
{
    private const string AccountId = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";

    private static ModifyAccountBody ABody(
        string? id = AccountId, string? name = "Amex Gold", string? type = "creditLine", string? openDate = "2024-06-15",
        string? openBalance = "99.50", string? description = "Travel card") =>
        new(id, name, type, openDate, openBalance, description);

    [Fact]
    public void ToModifyAccountRequest_ReturnsTheChangesForTheUser()
    {
        var user = AUser();

        var result = ModifyAccountRequestRaw.ToModifyAccountRequest(new(ABody()), user);

        result.Should().BeSuccessful().And.WithValue(new ModifyAccountRequest(
            user, Id.Valid(Guid.Parse(AccountId)), AccountName.Valid("Amex Gold"), AccountType.CreditLine, new DateOnly(2024, 6, 15),
            Money.Valid(99.50m), "Travel card"));
    }

    [Fact]
    public void ToModifyAccountRequest_LeavesMissingTypeOpenDateOpenBalanceAndDescriptionUnchanged()
    {
        var result = ModifyAccountRequestRaw.ToModifyAccountRequest(
            new(ABody(type: null, openDate: null, openBalance: null, description: null)), AUser());

        result.Should().BeSuccessful();
        result.Value.Type.Should().BeNull();
        result.Value.OpenDate.Should().BeNull();
        result.Value.OpenBalance.Should().BeNull();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public void ToModifyAccountRequest_Fails_WithEveryInvalidField()
    {
        var result = ModifyAccountRequestRaw.ToModifyAccountRequest(
            new(new ModifyAccountBody(Guid.Empty.ToString(), "", "wallet", "0001-01-01", "12.345", null)), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys.Should().BeEquivalentTo(
            IdErrors.CodeInvalid,
            AccountNameErrors.CodeInvalid,
            AccountTypeErrors.CodeInvalid,
            OpenDateErrors.CodeInvalid,
            MoneyErrors.CodeInvalid);
    }
}
