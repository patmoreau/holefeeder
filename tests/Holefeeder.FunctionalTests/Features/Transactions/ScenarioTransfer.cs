using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Application.Features.Transactions.Commands.Transfer;
using static Holefeeder.Tests.Common.Builders.Transactions.TransferRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public sealed class ScenarioTransfer(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task InvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.MakesATransfer)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task ValidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Category.TransferCategoriesExists)
            .And(Account.CollectionExists)
            .And(AValidRequest)
            .When(TheUser.MakesATransfer)
            .Then(Transaction.ShouldBeCreatedForBothAccounts)
            .PlayAsync();

    [Fact]
    public Task ValidRequestWithNoDescription() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Category.TransferCategoriesExists)
            .And(Account.CollectionExists)
            .And(AValidRequestWithNoDescription)
            .When(TheUser.MakesATransfer)
            .Then(Transaction.ShouldBeCreatedForBothAccountsWithTransferDescription)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() => GivenAnInvalidTransfer().Build());

    private static void AValidRequest(IStepRunner runner) =>
        runner.Execute<IEnumerable<Account>, Request>(accounts =>
        {
            accounts.Should().BeValid()
                .And.Subject.Value.Should().HaveCountGreaterThan(1);
            var fromAccount = accounts.Value.First();
            var toAccount = accounts.Value.Last();
            var request = GivenATransfer()
                .FromAccount(fromAccount)
                .ToAccount(toAccount)
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void AValidRequestWithNoDescription(IStepRunner runner) =>
        runner.Execute<IEnumerable<Account>, Request>(accounts =>
        {
            accounts.Should().BeValid()
                .And.Subject.Value.Should().HaveCountGreaterThan(1);
            var fromAccount = accounts.Value.First();
            var toAccount = accounts.Value.Last();
            var request = GivenATransfer()
                .FromAccount(fromAccount)
                .ToAccount(toAccount)
                .WithNoDescription()
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });
}
