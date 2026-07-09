using System.Net;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.Testing.Attributes;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

using static Holefeeder.Application.Features.Transactions.Commands.ModifyTransaction;
using static Holefeeder.Tests.Common.Builders.Transactions.ModifyTransactionRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioModifyTransaction(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.ModifiesATransaction)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task WhenAccountDoesNotExists() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Transaction.Exists)
            .And(ARequestWithNonExistentAccount)
            .When(TheUser.ModifiesATransaction)
            .Then(ShouldReceiveAnAccountNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenCategoryDoesNotExists() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Transaction.Exists)
            .And(ARequestWithNonExistentCategory)
            .When(TheUser.ModifiesATransaction)
            .Then(ShouldReceiveACategoryNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenTransactionDoesNotExists() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(ARequestWithNonExistentTransaction)
            .When(TheUser.ModifiesATransaction)
            .Then(ShouldNotBeFound)
            .PlayAsync();

    [Fact]
    public Task WhenModifyATransaction() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Transaction.Exists)
            .And(AValidRequest)
            .When(TheUser.ModifiesATransaction)
            .Then(Transaction.ShouldBeModified)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() =>
        {
            var request = GivenAnInvalidModifyTransactionRequest().Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void ARequestWithNonExistentAccount(IStepRunner runner) =>
        runner.Execute<Transaction, Request>(transaction =>
        {
            transaction.Should().BeValid();
            var request = GivenAModifyTransactionRequest()
                .WithId(transaction.Value.Id)
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void ARequestWithNonExistentCategory(IStepRunner runner) =>
        runner.Execute(() =>
        {
            var account = runner.GetContextData<Account>(AccountContext.ExistingAccount);
            var request = GivenAModifyTransactionRequest()
                .WithAccount(account)
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void ARequestWithNonExistentTransaction(IStepRunner runner) =>
        runner.Execute(() =>
        {
            var account = runner.GetContextData<Account>(AccountContext.ExistingAccount);
            var category = runner.GetContextData<Category>(CategoryContext.ExistingCategory);
            var request = GivenAModifyTransactionRequest()
                .WithAccount(account)
                .WithCategory(category)
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void AValidRequest(IStepRunner runner) =>
        runner.Execute<Transaction, Request>(transaction =>
        {
            transaction.Should().BeValid();
            var request = GivenAModifyTransactionRequest()
                .WithId(transaction.Value.Id)
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    [AssertionMethod]
    private static void ShouldReceiveAnAccountNotFound(IStepRunner runner) =>
        runner.Execute<IApiResponse>(response =>
        {
            var request = runner.GetContextData<Request>(RequestContext.CurrentRequest);
            response.Should().BeValid()
                .And.Subject
                .Value.Should().BeFailure()
                .And.HaveStatusCode(HttpStatusCode.BadRequest)
                .And.HaveError($"Account '{request.AccountId.Value}' does not exists.");
        });

    [AssertionMethod]
    private static void ShouldReceiveACategoryNotFound(IStepRunner runner) =>
        runner.Execute<IApiResponse>(response =>
        {
            var request = runner.GetContextData<Request>(RequestContext.CurrentRequest);
            response.Should().BeValid()
                .And.Subject
                .Value.Should().BeFailure()
                .And.HaveStatusCode(HttpStatusCode.BadRequest)
                .And.HaveError($"Category '{request.CategoryId.Value}' does not exists.");
        });
}
