using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Api.ModifyCashflow;
using Holefeeder.Service.Cashflows.Core.ModifyCashflow;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Cashflows.Api.ModifyCashflow;

public class ModifyCashflowRequestRawTests
{
    private static ModifyCashflowBody ABody(
        string? id = "3f2504e0-4f89-11d3-9a0c-0305e82c3301", string? amount = "1500.00", string? effectiveDate = "2026-02-01",
        string? description = "Rent", string?[]? tags = null) =>
        new(id, amount, effectiveDate, description, tags ?? ["Home", "rent"]);

    [Fact]
    public void ToModifyCashflowRequest_ReturnsTheModificationForTheUser_WhenValid()
    {
        var user = AUser();

        var result = ModifyCashflowRequestRaw.ToModifyCashflowRequest(new(ABody()), user);

        result.Should().BeSuccessful().And.WithValue(new ModifyCashflowRequest(
            user, Id.Valid(Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301")), Money.Valid(1500m), new DateOnly(2026, 2, 1), "Rent",
            TagList.Create(["home", "rent"])));
    }

    [Fact]
    public void ToModifyCashflowRequest_KeepsNoDescription_WhenItIsMissing() =>
        ModifyCashflowRequestRaw.ToModifyCashflowRequest(new(ABody(description: null)), AUser()).Value.Description.Should().BeNull();

    [Fact]
    public void ToModifyCashflowRequest_FailsValidation_WhenIdIsInvalid() =>
        ValidationErrorsOf(ModifyCashflowRequestRaw.ToModifyCashflowRequest(new(ABody(id: Guid.Empty.ToString())), AUser()))
            .Should().ContainKey(IdErrors.CodeInvalid);

    [Fact]
    public void ToModifyCashflowRequest_FailsValidation_WhenAmountIsInvalid() =>
        ValidationErrorsOf(ModifyCashflowRequestRaw.ToModifyCashflowRequest(new(ABody(amount: "-5")), AUser()))
            .Should().ContainKey(MoneyErrors.CodeInvalid);

    [Fact]
    public void ToModifyCashflowRequest_FailsValidation_WhenEffectiveDateIsInvalid() =>
        ValidationErrorsOf(ModifyCashflowRequestRaw.ToModifyCashflowRequest(new(ABody(effectiveDate: null)), AUser()))
            .Should().ContainKey(ModifyCashflowRequestErrors.CodeEffectiveDateInvalid);

    [Fact]
    public void ToModifyCashflowRequest_ReportsEveryError_WhenEverythingIsInvalid() =>
        ValidationErrorsOf(ModifyCashflowRequestRaw.ToModifyCashflowRequest(new(new ModifyCashflowBody(null, null, null, null, null)), AUser()))
            .Should().ContainKeys(IdErrors.CodeInvalid, MoneyErrors.CodeInvalid, ModifyCashflowRequestErrors.CodeEffectiveDateInvalid);

    [Fact]
    public void Errors_FollowTheServiceConventions()
    {
        ModifyCashflowRequestErrors.EffectiveDateInvalid.Code.Should().Be("cashflow-effective-date-invalid");
        ModifyCashflowRequestErrors.EffectiveDateInvalid.Description.Should().Be("Effective date must be a date as YYYY-MM-DD.");
    }

    private static IReadOnlyDictionary<string, string[]> ValidationErrorsOf(Result<ModifyCashflowRequest> result)
    {
        result.Should().BeFailure();
        return result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors;
    }
}
