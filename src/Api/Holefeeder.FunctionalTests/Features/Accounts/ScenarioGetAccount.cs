using System.Net;

using FluentAssertions;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using Xunit;
using Xunit.Abstractions;

using static Holefeeder.Tests.Common.Builders.AccountEntityBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.TransactionEntityBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioGetAccount : BaseScenario
{
    private readonly HolefeederDatabaseDriver _holefeederDatabaseDriver;
    private readonly BudgetingDatabaseDriver _budgetingDatabaseDriver;

    public ScenarioGetAccount(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        _holefeederDatabaseDriver = HolefeederDatabaseDriver;
        _budgetingDatabaseDriver = BudgetingDatabaseDriver;
        _holefeederDatabaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task WhenNotFound()
    {
        GivenUserIsAuthorized();

        await WhenUserGetAccount(Guid.NewGuid());

        ThenShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await WhenUserGetAccount(Guid.Empty);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        GivenUserIsAuthorized();

        await WhenUserGetAccount(Guid.NewGuid());

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        GivenForbiddenUserIsAuthorized();

        await WhenUserGetAccount(Guid.NewGuid());

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserGetAccount(Guid.NewGuid());

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenAccountExistsWithExpenses()
    {
        var account = await GivenAnActiveAccount()
            .OfType(AccountType.Checking)
            .ForUser(AuthorizedUserId)
            .SavedInDb(_holefeederDatabaseDriver);

        var category = await GivenACategory()
            .OfType(CategoryType.Expense)
            .ForUser(AuthorizedUserId)
            .SavedInDb(_budgetingDatabaseDriver);

        var transaction = await GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .SavedInDb(_holefeederDatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetAccount(account.Id);

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<AccountViewModel>();
        ThenAssertAll(() =>
        {
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(account, options => options.Excluding(x => x.UserId));
            result!.TransactionCount.Should().Be(1);
            result.Balance.Should().Be(account.OpenBalance - transaction.Amount);
        });
    }

    [Fact]
    public async Task WhenAccountExistsWithGains()
    {
        var account = await GivenAnActiveAccount()
            .OfType(AccountType.Checking)
            .ForUser(AuthorizedUserId)
            .SavedInDb(_holefeederDatabaseDriver);

        var category = await GivenACategory()
            .OfType(CategoryType.Gain)
            .ForUser(AuthorizedUserId)
            .SavedInDb(_budgetingDatabaseDriver);

        var transaction = await GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .SavedInDb(_holefeederDatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetAccount(account.Id);

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<AccountViewModel>();
        ThenAssertAll(() =>
        {
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(account, options => options.Excluding(x => x.UserId));
            result!.TransactionCount.Should().Be(1);
            result.Balance.Should().Be(account.OpenBalance + transaction.Amount);
        });
    }

    private async Task WhenUserGetAccount(Guid id)
    {
        await HttpClientDriver.SendGetRequest(ApiResources.GetAccount, new object?[] {id.ToString()});
    }
}
