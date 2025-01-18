using System.Net;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.Transactions.Commands;
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
            var account = runner.GetContextData<Account>(AccountContexts.ExistingAccount);
            account.Should().NotBeNull();

            var category = runner.GetContextData<Category>(CategoryContexts.ExistingCategory);
            category.Should().NotBeNull();

            var cashflow = await GivenAnActiveCashflow()
                .ForAccount(account)
                .ForCategory(category)
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(CashflowContexts.ExistingCashflow, cashflow);
            return cashflow;
        });

    public void CollectionExists(IStepRunner runner) =>
        runner.Execute("the user has multiple cashflows", async () =>
        {
            var account = runner.GetContextData<Account>(AccountContexts.ExistingAccount);
            account.Should().NotBeNull();

            var category = runner.GetContextData<Category>(CategoryContexts.ExistingCategory);
            category.Should().NotBeNull();

            var cashflows = await GivenAnActiveCashflow()
                .ForAccount(account)
                .ForCategory(category)
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .CollectionSavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(CashflowContexts.ExistingCashflows, cashflows);

            return cashflows;
        });

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
}
