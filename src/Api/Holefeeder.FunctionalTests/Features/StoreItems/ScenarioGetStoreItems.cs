using System.Net;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.StoreItems.StoreItemBuilder;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

public class ScenarioGetStoreItems : BaseScenario
{
    public ScenarioGetStoreItems(ApiApplicationDriver applicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetStoreItems, -1);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountsExists()
    {
        const string firstCode = nameof(firstCode);
        const string secondCode = nameof(secondCode);

        await GivenAStoreItem()
            .ForUser(HolefeederUserId)
            .WithCode(firstCode)
            .SavedInDb(DatabaseDriver);

        await GivenAStoreItem()
            .ForUser(HolefeederUserId)
            .WithCode(secondCode)
            .SavedInDb(DatabaseDriver);

        await GivenAStoreItem()
            .SavedInDb(DatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetStoreItems, sorts: "-code");

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        StoreItemViewModel[]? result = HttpClientDriver.DeserializeContent<StoreItemViewModel[]>();
        ThenAssertAll(() =>
        {
            result.Should().NotBeNull().And.HaveCount(2);
            result![0].Code.Should().Be(secondCode);
            result[1].Code.Should().Be(firstCode);
        });
    }
}
