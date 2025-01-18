using System.Net;
using System.Text.Json;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.Tests.Common;

using Refit;

using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed class TransactionSteps(BudgetingDatabaseDriver budgetingDatabaseDriver)
{
    public void Exists(IStepRunner runner) =>
        runner.Execute("a user has a transaction", async () =>
        {
            var account = runner.GetContextData<Account>(AccountContexts.ExistingAccount);
            account.Should().NotBeNull();

            var category = runner.GetContextData<Category>(CategoryContexts.ExistingCategory);
            category.Should().NotBeNull();

            var transaction = await GivenATransaction()
                .ForAccount(account)
                .ForCategory(category)
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(TransactionContexts.ExistingTransaction, transaction);
            return transaction;
        });

    public void CollectionExists(IStepRunner runner) =>
        runner.Execute("the user has multiple cashflows", async () =>
        {
            var account = runner.GetContextData<Account>(AccountContexts.ExistingAccount);
            account.Should().NotBeNull();

            var category = runner.GetContextData<Category>(CategoryContexts.ExistingCategory);
            category.Should().NotBeNull();

            var transactions = await GivenATransaction()
                .ForAccount(account)
                .ForCategory(category)
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .CollectionSavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(TransactionContexts.ExistingTransactions, transactions);

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
}
