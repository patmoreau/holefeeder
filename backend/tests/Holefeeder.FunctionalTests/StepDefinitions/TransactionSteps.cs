using System.Net;

using Bogus;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.Application.UseCases;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed class TransactionSteps(BudgetingDatabaseDriver budgetingDatabaseDriver)
{
    public void Exists(IStepRunner runner) =>
        runner.Execute("a user has a transaction", async () =>
        {
            var account = runner.GetContextData<Account>(AccountContext.ExistingAccount);
            account.Should().NotBeNull();

            var category = runner.GetContextData<Category>(CategoryContext.ExistingCategory);
            category.Should().NotBeNull();

            var transaction = await GivenATransaction()
                .ForAccount(account)
                .ForCategory(category)
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(TransactionContext.ExistingTransaction, transaction);
            return transaction;
        });

    public void CollectionExists(IStepRunner runner) =>
        runner.Execute("the user has multiple cashflows", async () =>
        {
            var account = runner.GetContextData<Account>(AccountContext.ExistingAccount);
            account.Should().NotBeNull();

            var category = runner.GetContextData<Category>(CategoryContext.ExistingCategory);
            category.Should().NotBeNull();

            var transactions = await GivenATransaction()
                .ForAccount(account)
                .ForCategory(category)
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .CollectionSavedInDbAsync(budgetingDatabaseDriver, new Faker().Random.Int(2, 10));

            runner.SetContextData(TransactionContext.ExistingTransactions, transactions);

            return transactions;
        });

    [AssertionMethod]
    public void ShouldNotBeFound(IStepRunner runner) =>
        runner.Execute<IApiResponse, Task>("the transaction should not be found", async response =>
        {
            var request = runner.GetContextData<DeleteTransaction.Request>(RequestContext.CurrentRequest);

            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.NoContent);

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Transactions.FindAsync(request.Id);
            result.Should().BeNull();
        });

    [AssertionMethod]
    internal void ShouldBeModified(IStepRunner runner) =>
        runner.Execute<IApiResponse, Task>("the transaction should be modified", async response =>
        {
            var request = runner.GetContextData<ModifyTransaction.Request>(RequestContext.CurrentRequest);

            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.NoContent);

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Transactions.FindAsync(request.Id);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        });

    [AssertionMethod]
    internal void ShouldBeSynced(IStepRunner runner) =>
        runner.Execute<IApiResponse>("the transaction should be synced", async response =>
        {
            var request = runner.GetContextData<PowerSync.Request>(RequestContext.CurrentRequest);

            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.NoContent);

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Transactions.FindAsync((TransactionId)request.Operations.First().Id);

            if (request.Operations.First().Op == "DELETE")
            {
                result.Should().BeNull();
                return;
            }

            var expected = runner.GetContextData<Transaction>(PowerSyncContext.SyncData);
            result.Should().NotBeNull();
            result.Id.Should().Be(expected.Id);
            result.Date.Should().Be(expected.Date);
            result.Amount.Should().Be(expected.Amount);
            result.Description.Should().Be(expected.Description);
            result.AccountId.Should().Be(expected.AccountId);
            result.CategoryId.Should().Be(expected.CategoryId);
            if (expected.CashflowId == null)
                result.CashflowId.Should().BeNull();
            else
                result.CashflowId.Should().Be(expected.CashflowId);
            if (expected.CashflowDate == null)
                result.CashflowDate.Should().BeNull();
            else
                result.CashflowDate.Should().Be(expected.CashflowDate);
            result.Tags.Should().BeEquivalentTo(expected.Tags);
        });
}
