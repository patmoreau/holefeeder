using System.Net;
using System.Text.Json;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.Application.Features.Accounts.Commands.OpenAccount;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Accounts.OpenAccountRequestBuilder;
using static Holefeeder.Tests.Common.SeedWork.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioOpenAccount : BaseScenario
{
    public ScenarioOpenAccount(ApiApplicationDriver applicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        Request request = GivenAnInvalidOpenAccountRequest()
            .Build();

        GivenUserIsAuthorized();

        await WhenUserOpensAnAccount(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountNameAlreadyExistsRequest()
    {
        Account entity = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        Request request = GivenAnOpenAccountRequest()
            .WithName(entity.Name)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserOpensAnAccount(request);

        ThenShouldExpectStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenOpenAccount()
    {
        Request request = GivenAnOpenAccountRequest()
            .Build();

        GivenUserIsAuthorized();

        await WhenUserOpensAnAccount(request);

        ThenShouldExpectStatusCode(HttpStatusCode.Created);

        Guid id = ThenShouldGetTheRouteOfTheNewResourceInTheHeader();

        Account? result = await DatabaseDriver.FindByIdAsync<Account>(id);
        result.Should().NotBeNull().And.BeEquivalentTo(request, options => options.ExcludingMissingMembers());
    }

    private async Task WhenUserOpensAnAccount(Request request)
    {
        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.OpenAccount, json);
    }
}
