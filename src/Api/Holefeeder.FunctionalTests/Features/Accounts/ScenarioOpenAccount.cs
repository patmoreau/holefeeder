using System.Net;
using System.Text.Json;
using DrifterApps.Seeds.Testing.Scenarios;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.Application.Features.Accounts.Commands.OpenAccount;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Accounts.OpenAccountRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

[ComponentTest]
public class ScenarioOpenAccount : HolefeederScenario
{
    private const string OpenAccountRequestKey = $"{nameof(ScenarioOpenAccount)}_OpenAccountRequest";

    public ScenarioOpenAccount(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
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
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        Request request = GivenAnOpenAccountRequest()
            .WithName(entity.Name)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserOpensAnAccount(request);

        ShouldExpectStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenOpenAccount() => await ScenarioFor("opening an account", runner =>
        runner.Given(User.IsAuthorized)
            .When(OpeningAccount)
            .Then(AccountShouldBeOpened));

    private void OpeningAccount(IStepRunner runner)
    {
        runner.Execute("opening an account", async () =>
        {
            var request = GivenAnOpenAccountRequest().Build();

            runner.SetContextData(OpenAccountRequestKey, request);

            await WhenUserOpensAnAccount(request);
        });
    }

    private async Task WhenUserOpensAnAccount(Request request)
    {
        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequestAsync(ApiResources.OpenAccount, json);
    }

    private void AccountShouldBeOpened(IStepRunner runner)
    {
        runner.Execute("the account was created successfully", async () =>
        {
            ShouldExpectStatusCode(HttpStatusCode.Created);

            Uri location = ShouldGetTheRouteOfTheNewResourceInTheHeader();
            var id = ResourceIdFromLocation(location);

            Account? result = await DatabaseDriver.FindByIdAsync<Account>(id);

            var request = runner.GetContextData<Request>(OpenAccountRequestKey);

            result.Should().NotBeNull().And.BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        });
    }
}
