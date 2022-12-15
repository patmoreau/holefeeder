using System.Net;

using Holefeeder.Application.Features.Transactions;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;

using static Holefeeder.Application.Features.Transactions.Commands.Transfer;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransferRequestBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public sealed class ScenarioTransfer : BaseScenario<ScenarioTransfer>
{

    public ScenarioTransfer(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper) : base(
        apiApplicationDriver, testOutputHelper)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }

        BudgetingDatabaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task InvalidRequest()
    {
        Request request = default!;
        await Given(() =>
            {
                request = GivenAnInvalidTransfer().Build();
            })
            .Given(() => User.IsAuthorized())
            .When(() => Transaction.Transfer(request))
            .Then(
                () => ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred."))
            .RunScenarioAsync();
    }

    [Fact]
    public async Task AuthorizedUser()
    {
        Request request = null!;

        await Given(() => request = GivenATransfer().Build())
            .Given(() => User.IsAuthorized())
            .When(() => Transaction.Transfer(request))
            .Then(ThenUserShouldBeAuthorizedToAccessEndpoint)
            .RunScenarioAsync();
    }

    [Fact]
    public async Task ForbiddenUser()
    {
        Request request = null!;
        await Given(() => request = GivenATransfer().Build())
            .Given(() => User.IsForbidden())
            .When(() => Transaction.Transfer(request))
            .Then(ShouldBeForbiddenToAccessEndpoint)
            .RunScenarioAsync();
    }

    [Fact]
    public async Task UnauthorizedUser()
    {
        Request entity = null!;
        await Given(() => entity = GivenATransfer().Build())
            .Given(() => User.IsUnauthorized())
            .When(() => Transaction.Transfer(entity))
            .Then(ShouldNotBeAuthorizedToAccessEndpoint)
            .RunScenarioAsync();
    }

    [Fact]
    public async Task ValidRequest()
    {
        Account fromAccount = null!;
        Account toAccount = null!;
        Request request = null!;
        (Guid FromTransactionId, Guid ToTransactionId) ids = default;

        await Given(async () => fromAccount = await GivenAnActiveAccount()
                .ForUser(AuthorizedUserId)
                .SavedInDb(BudgetingDatabaseDriver))
            .Given(async () => toAccount = await GivenAnActiveAccount()
                .ForUser(AuthorizedUserId)
                .SavedInDb(BudgetingDatabaseDriver))
            .Given(async () => await GivenACategory()
                .WithName("Transfer In")
                .ForUser(AuthorizedUserId)
                .SavedInDb(BudgetingDatabaseDriver))
            .Given(async () => await GivenACategory()
                .WithName("Transfer Out")
                .ForUser(AuthorizedUserId)
                .SavedInDb(BudgetingDatabaseDriver))
            .Given(() => request = GivenATransfer()
                .FromAccount(fromAccount)
                .ToAccount(toAccount)
                .Build())
            .Given(() => User.IsAuthorized())
            .When(() => Transaction.Transfer(request))
            .Then(() => ThenShouldExpectStatusCode(HttpStatusCode.Created))
            .Then(() => ThenShouldGetTheRouteOfTheNewResourceInTheHeader())
            .Then(() => ids = ThenShouldReceive<(Guid FromTransactionId, Guid ToTransactionId)>())
            .Then(async () =>
            {
                var result = await BudgetingDatabaseDriver.FindByIdAsync<Transaction>(ids.FromTransactionId);

                TransactionMapper.MapToModelOrNull(result).Should()
                    .NotBeNull()
                    .And
                    .BeEquivalentTo(request, options => options.ExcludingMissingMembers());
            })
            .Then(async () =>
            {
                var result = await BudgetingDatabaseDriver.FindByIdAsync<Transaction>(ids.ToTransactionId);

                TransactionMapper.MapToModelOrNull(result).Should()
                    .NotBeNull()
                    .And
                    .BeEquivalentTo(request, options => options.ExcludingMissingMembers());
            })
            .RunScenarioAsync();
    }
}
