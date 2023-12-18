using System.Net;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.StoreItems.StoreItemBuilder;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetStoreItems : HolefeederScenario
{
    public ScenarioGetStoreItems(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await QueryEndpoint(ApiResources.GetStoreItems, -1);

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
            .SavedInDbAsync(DatabaseDriver);

        await GivenAStoreItem()
            .ForUser(HolefeederUserId)
            .WithCode(secondCode)
            .SavedInDbAsync(DatabaseDriver);

        await GivenAStoreItem()
            .SavedInDbAsync(DatabaseDriver);

        GivenUserIsAuthorized();

        await QueryEndpoint(ApiResources.GetStoreItems, sorts: "-code");

        ShouldExpectStatusCode(HttpStatusCode.OK);
        StoreItemViewModel[]? result = HttpClientDriver.DeserializeContent<StoreItemViewModel[]>();
        AssertAll(() =>
        {
            result.Should().NotBeNull().And.HaveCount(2);
            result![0].Code.Should().Be(secondCode);
            result[1].Code.Should().Be(firstCode);
        });
    }
}
