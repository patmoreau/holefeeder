using System.Net;
using System.Text.Json;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Application.Features.Accounts.Commands.ModifyAccount;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Accounts.ModifyAccountRequestBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioModifyAccount : BaseScenario
{
    public ScenarioModifyAccount(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        var entity = GivenAnInvalidModifyAccountRequest()
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifiesAccount(entity);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountNotFound()
    {
        var request = GivenAModifyAccountRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserModifiesAccount(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var entity = GivenAModifyAccountRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserModifiesAccount(entity);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var entity = GivenAModifyAccountRequest().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserModifiesAccount(entity);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var entity = GivenAModifyAccountRequest().Build();

        GivenUserIsUnauthorized();

        await WhenUserModifiesAccount(entity);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenModifyAccount()
    {
        var entity = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        var request = GivenAModifyAccountRequest()
            .WithId(entity.Id)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifiesAccount(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        var result = await DatabaseDriver.FindByIdAsync<Account>(entity.Id);
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(request, options => options.ExcludingMissingMembers());
    }

    private async Task WhenUserModifiesAccount(Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.ModifyAccount, json);
    }
}
