using System.Net;
using System.Text.Json;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.Application.Features.Accounts.Commands.ModifyAccount;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Accounts.ModifyAccountRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioModifyAccount : BaseScenario
{
    public ScenarioModifyAccount(ApiApplicationDriver applicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        Request entity = GivenAnInvalidModifyAccountRequest()
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifiesAccount(entity);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountNotFound()
    {
        Request request = GivenAModifyAccountRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserModifiesAccount(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenModifyAccount()
    {
        Account entity = await GivenAnActiveAccount()
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        Request request = GivenAModifyAccountRequest()
            .WithId(entity.Id)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifiesAccount(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        Account? result = await DatabaseDriver.FindByIdAsync<Account>(entity.Id);
        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(request, options => options.ExcludingMissingMembers());
    }

    private async Task WhenUserModifiesAccount(Request request)
    {
        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.ModifyAccount, json);
    }
}
