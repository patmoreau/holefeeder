using System.Net;
using System.Text.Json;

using Bogus;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;
using DrifterApps.Seeds.Testing.Extensions;

using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.Tests.Common;

using Microsoft.EntityFrameworkCore;

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
    public void ShouldBeCreated(IStepRunner runner) =>
        runner.Execute<IApiResponse>("the new transaction should be created", async response =>
        {
            var request = runner.GetContextData<MakePurchase.Request>(RequestContext.CurrentRequest);

            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.Created);

            response.Value.Headers.Location.Should().NotBeNull();

            var id = (TransactionId)response.Value.Headers.Location!.ExtractGuidFromUrl();

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Transactions.FindAsync(id);
            result.Should().NotBeNull().And.BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        });

    [AssertionMethod]
    public void ShouldBeCreatedFromCashflowPayment(IStepRunner runner) =>
        runner.Execute<IApiResponse>("the new transaction should be created from cashflow payment", async response =>
        {
            var request = runner.GetContextData<PayCashflow.Request>(RequestContext.CurrentRequest);

            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.Created);

            response.Value.Headers.Location.Should().NotBeNull();

            var id = (TransactionId)response.Value.Headers.Location!.ExtractGuidFromUrl();

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Transactions.FindAsync(id);
            result.Should().NotBeNull().And.BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        });

    [AssertionMethod]
    public void ShouldBeCreatedForBothAccounts(IStepRunner runner) =>
        runner.Execute<IApiResponse>("the new transactions should be created from the transfer", async response =>
        {
            var request = runner.GetContextData<Transfer.Request>(RequestContext.CurrentRequest);
            var categories = runner.GetContextData<IEnumerable<Category>>(CategoryContext.ExistingCategories).ToList();

            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.Created);

            response.Value.Headers.Location.Should().NotBeNull();

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Transactions.FirstOrDefaultAsync(x => x.AccountId == request.FromAccountId);
            result.Should().NotBeNull().And.BeEquivalentTo(request, options =>
                options.ExcludingMissingMembers());
            result!.CategoryId.Should().Be(categories.First().Id);

            result = await dbContext.Transactions.FirstOrDefaultAsync(x => x.AccountId == request.ToAccountId);
            result.Should().NotBeNull().And.BeEquivalentTo(request, options => options.ExcludingMissingMembers());
            result!.CategoryId.Should().Be(categories.Last().Id);
        });
}
