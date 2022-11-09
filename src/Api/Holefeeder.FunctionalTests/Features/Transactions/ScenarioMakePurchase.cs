using System.Net;

using FluentAssertions;

using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Infrastructure.Mapping;

using Xunit;
using Xunit.Abstractions;

using static Holefeeder.Tests.Common.Builders.AccountEntityBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.TransactionBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public sealed class ScenarioMakePurchase : BaseScenario<ScenarioMakePurchase>
{
    private readonly HolefeederDatabaseDriver _databaseDriver;
    private readonly BudgetingDatabaseDriver _budgetingDatabaseDriver;

    public ScenarioMakePurchase(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper) : base(
        apiApplicationDriver, testOutputHelper)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }

        _databaseDriver = apiApplicationDriver.CreateHolefeederDatabaseDriver();
        _budgetingDatabaseDriver = apiApplicationDriver.CreateBudgetingDatabaseDriver();
        _databaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task InvalidRequest()
    {
        Transaction entity = default!;
        await Given(() =>
            {
                entity = ATransaction()
                    .OfAmount(0)
                    .Build();
            })
            .Given(() => User.IsAuthorized())
            .When(() => Transaction.MakesPurchase(entity))
            .Then(
                () => ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred."))
            .RunScenarioAsync();
    }

    [Fact]
    public async Task AuthorizedUser()
    {
        Transaction entity = null!;

        await Given(() => entity = ATransaction().Build())
            .Given(() => User.IsAuthorized())
            .When(() => Transaction.MakesPurchase(entity))
            .Then(ThenUserShouldBeAuthorizedToAccessEndpoint)
            .RunScenarioAsync();
    }

    [Fact]
    public async Task ForbiddenUser()
    {
        Transaction entity = null!;
        await Given(() => entity = ATransaction().Build())
            .Given(() => User.IsForbidden())
            .When(() => Transaction.MakesPurchase(entity))
            .Then(ShouldBeForbiddenToAccessEndpoint)
            .RunScenarioAsync();
    }

    [Fact]
    public async Task UnauthorizedUser()
    {
        Transaction entity = null!;
        await Given(() => entity = ATransaction().Build())
            .Given(() => User.IsUnauthorized())
            .When(() => Transaction.MakesPurchase(entity))
            .Then(ShouldNotBeAuthorizedToAccessEndpoint)
            .RunScenarioAsync();
    }

    [Fact]
    public async Task ValidRequest()
    {
        AccountEntity account = null!;
        Category category = null!;
        Transaction entity = null!;
        var id = Guid.Empty;

        await Given(async () => account = await GivenAnActiveAccount()
                .ForUser(AuthorizedUserId)
                .SavedInDb(_databaseDriver))
            .Given(async () => category = await GivenACategory()
                .ForUser(AuthorizedUserId)
                .SavedInDb(_budgetingDatabaseDriver))
            .Given(() => entity = ATransaction()
                .ForAccount(account)
                .ForCategory(category)
                .ForUser(AuthorizedUserId)
                .Build())
            .Given(() => User.IsAuthorized())
            .When(() => Transaction.MakesPurchase(entity))
            .Then(() => ThenShouldExpectStatusCode(HttpStatusCode.Created))
            .Then(() => id = ThenShouldGetTheRouteOfTheNewResourceInTheHeader())
            .Then(async () =>
            {
                var result = await _databaseDriver.FindByIdAsync<TransactionEntity>(id, entity.UserId);

                TransactionMapper.MapToModelOrNull(result).Should()
                    .NotBeNull()
                    .And
                    .BeEquivalentTo(entity, options => options.Excluding(info => info.Id));
            })
            .RunScenarioAsync();
    }
}
