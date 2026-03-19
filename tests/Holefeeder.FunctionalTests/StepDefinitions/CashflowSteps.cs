using System.Net;

using Bogus;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.Application.UseCases;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed class CashflowSteps(BudgetingDatabaseDriver budgetingDatabaseDriver)
{
    public void Exists(IStepRunner runner) =>
        runner.Execute("a user has an active cashflow", async () =>
        {
            var account = runner.GetContextData<Account>(AccountContext.ExistingAccount);
            account.Should().NotBeNull();

            var category = runner.GetContextData<Category>(CategoryContext.ExistingCategory);
            category.Should().NotBeNull();

            var cashflow = await GivenAnActiveCashflow()
                .ForAccount(account)
                .ForCategory(category)
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(CashflowContext.ExistingCashflow, cashflow);
            return cashflow;
        });

    public void CollectionExists(IStepRunner runner) =>
        runner.Execute("the user has multiple cashflows", async () =>
        {
            var account = runner.GetContextData<Account>(AccountContext.ExistingAccount);
            account.Should().NotBeNull();

            var category = runner.GetContextData<Category>(CategoryContext.ExistingCategory);
            category.Should().NotBeNull();

            var cashflows = await GivenAnActiveCashflow()
                .ForAccount(account)
                .ForCategory(category)
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .CollectionSavedInDbAsync(budgetingDatabaseDriver, new Faker().Random.Int(2, 10));

            runner.SetContextData(CashflowContext.ExistingCashflows, cashflows);

            return cashflows;
        });

    public void AnActiveOneTime(IStepRunner runner) =>
        runner.Execute("a user has an active one time cashflow",
            () => BuildCashflow(runner, DateIntervalType.OneTime, 1));

    public void AnActiveBiWeekly(IStepRunner runner) =>
        runner.Execute("a user has an active biweekly cashflow",
            () => BuildCashflow(runner, DateIntervalType.Weekly, 2));

    public void AnActiveMonthly(IStepRunner runner) =>
        runner.Execute("a user has an active monthly cashflow",
            () => BuildCashflow(runner, DateIntervalType.Monthly, 1));

    public void AnActiveYearly(IStepRunner runner) =>
        runner.Execute("a user has an active yearly cashflow",
            () => BuildCashflow(runner, DateIntervalType.Yearly, 1));

    private async Task<Cashflow> BuildCashflow(IStepRunner runner, DateIntervalType dateIntervalType, int frequency)
    {
        var account = runner.GetContextData<Account>(AccountContext.ExistingAccount);
        account.Should().NotBeNull();

        var category = runner.GetContextData<Category>(CategoryContext.ExistingCategory);
        category.Should().NotBeNull();

        var cashflow = await GivenAnActiveCashflow()
            .ForAccount(account)
            .ForCategory(category)
            .OfFrequency(dateIntervalType, frequency)
            .ForUser(TestUsers[AuthorizedUser].UserId)
            .SavedInDbAsync(budgetingDatabaseDriver);

        runner.SetContextData(CashflowContext.ExistingCashflow, cashflow);
        return cashflow;
    }

    [AssertionMethod]
    public void ShouldBeInactive(IStepRunner runner) =>
        runner.Execute<IApiResponse, Task>("the cashflow should be inactive", async response =>
        {
            var request = runner.GetContextData<CancelCashflow.Request>(RequestContext.CurrentRequest);

            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.NoContent);

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Cashflows.FindAsync(request.Id);
            result.Should().NotBeNull()
                .And.Subject.As<Cashflow>()
                .Inactive.Should().BeTrue();
        });

    [AssertionMethod]
    public void ShouldBeModified(IStepRunner runner) =>
        runner.Execute<IApiResponse, Task>("the cashflow should be modified", async response =>
        {
            var request = runner.GetContextData<ModifyCashflow.Request>(RequestContext.CurrentRequest);

            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.NoContent);

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Cashflows.FindAsync(request.Id);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        });

    [AssertionMethod]
    internal void ShouldBeSynced(IStepRunner runner) =>
        runner.Execute<IApiResponse>("the cashflow should be synced", async response =>
        {
            var request = runner.GetContextData<PowerSync.Request>(RequestContext.CurrentRequest);

            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.NoContent);

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Cashflows.FindAsync((CashflowId) request.Operations.First().Id);

            if (request.Operations.First().Op == "DELETE")
            {
                result.Should().BeNull();
                return;
            }

            var expected = runner.GetContextData<Cashflow>(PowerSyncContext.SyncData);
            result.Should().NotBeNull();
            result.Id.Should().Be(expected.Id);
            result.EffectiveDate.Should().Be(expected.EffectiveDate);
            result.IntervalType.Should().Be(expected.IntervalType);
            result.Frequency.Should().Be(expected.Frequency);
            result.Recurrence.Should().Be(expected.Recurrence);
            result.Amount.Should().Be(expected.Amount);
            result.Description.Should().Be(expected.Description);
            result.AccountId.Should().Be(expected.AccountId);
            result.CategoryId.Should().Be(expected.CategoryId);
            result.Tags.Should().Contain(expected.Tags);
        });
}
