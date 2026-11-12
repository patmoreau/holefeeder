using Holefeeder.Service.MyData.Core;

using static Holefeeder.ServiceTests.MyData.TestKit.MyDataForTest;

namespace Holefeeder.ServiceTests.MyData.Core;

public class MyDataRecordTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Account_IsInvalid_WithoutAName(string name) =>
        (AMyDataAccount() with { Name = name }).Validated().Should().BeFailure().And.WithError(MyDataRecordErrors.AccountNameInvalid);

    [Fact]
    public void Account_IsInvalid_WithANameLongerThan100Characters() =>
        (AMyDataAccount() with { Name = new string('a', 101) }).Validated().Should().BeFailure()
            .And.WithError(MyDataRecordErrors.AccountNameInvalid);

    [Fact]
    public void Account_IsInvalid_WithoutAnOpenDate() =>
        (AMyDataAccount() with { OpenDate = default }).Validated().Should().BeFailure()
            .And.WithError(MyDataRecordErrors.AccountOpenDateRequired);

    [Fact]
    public void Category_IsInvalid_WithoutAName() =>
        (AMyDataCategory() with { Name = " " }).Validated().Should().BeFailure().And.WithError(MyDataRecordErrors.CategoryNameInvalid);

    [Fact]
    public void Category_IsInvalid_WithANameLongerThan255Characters() =>
        (AMyDataCategory() with { Name = new string('a', 256) }).Validated().Should().BeFailure()
            .And.WithError(MyDataRecordErrors.CategoryNameInvalid);

    [Fact]
    public void Cashflow_IsInvalid_WithoutAnEffectiveDate() =>
        (AMyDataCashflow(AnId(), AnId()) with { EffectiveDate = default }).Validated().Should().BeFailure()
            .And.WithError(MyDataRecordErrors.CashflowEffectiveDateRequired);

    [Fact]
    public void Cashflow_IsInvalid_WithAFrequencyOfZero() =>
        (AMyDataCashflow(AnId(), AnId()) with { Frequency = 0 }).Validated().Should().BeFailure()
            .And.WithError(MyDataRecordErrors.CashflowFrequencyInvalid);

    [Fact]
    public void Cashflow_IsInvalid_WithANegativeRecurrence() =>
        (AMyDataCashflow(AnId(), AnId()) with { Recurrence = -1 }).Validated().Should().BeFailure()
            .And.WithError(MyDataRecordErrors.CashflowRecurrenceInvalid);

    [Fact]
    public void Transaction_IsInvalid_WithoutADate() =>
        (AMyDataTransaction(AnId(), AnId()) with { Date = default }).Validated().Should().BeFailure()
            .And.WithError(MyDataRecordErrors.TransactionDateRequired);

    [Fact]
    public void Records_AreValid_WhenEveryRuleHolds()
    {
        var account = AMyDataAccount();
        var category = AMyDataCategory();

        account.Validated().Should().BeSuccessful().And.WithValue(account);
        category.Validated().Should().BeSuccessful().And.WithValue(category);
        AMyDataCashflow(account.Id, category.Id).Validated().Should().BeSuccessful();
        AMyDataTransaction(account.Id, category.Id).Validated().Should().BeSuccessful();
    }
}
