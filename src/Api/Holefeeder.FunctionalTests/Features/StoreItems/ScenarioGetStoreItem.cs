using System.Net;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.StoreItems.StoreItemBuilder;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

public class ScenarioGetStoreItem : BaseScenario
{
    public ScenarioGetStoreItem(ApiApplicationDriver applicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenNotFound()
    {
        GivenUserIsAuthorized();

        await WhenUserGetStoreItem(Guid.NewGuid());

        ThenShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await WhenUserGetStoreItem(Guid.Empty);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenStoreItemExists()
    {
        StoreItem storeItem = await GivenAStoreItem()
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetStoreItem(storeItem.Id);

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        StoreItemViewModel? result = HttpClientDriver.DeserializeContent<StoreItemViewModel>();
        ThenAssertAll(() =>
        {
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(storeItem, options => options.Excluding(x => x.UserId).Excluding(x => x.DomainEvents));
        });
    }

    private async Task WhenUserGetStoreItem(Guid id) => await HttpClientDriver.SendGetRequest(ApiResources.GetStoreItem, new object[] { id.ToString() });
}
