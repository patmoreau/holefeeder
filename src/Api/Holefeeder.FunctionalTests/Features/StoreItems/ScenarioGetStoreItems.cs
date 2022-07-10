using System.Net;

using FluentAssertions;

using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using Xunit;
using Xunit.Abstractions;

using static Holefeeder.Tests.Common.Builders.StoreItemEntityBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

public class ScenarioGetStoreItems : BaseScenario
{
    private readonly ObjectStoreDatabaseDriver _objectStoreDatabaseDriver;

    public ScenarioGetStoreItems(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        _objectStoreDatabaseDriver = apiApplicationDriver.CreateObjectStoreDatabaseDriver();
        _objectStoreDatabaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetStoreItems, offset: -1);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        GivenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetStoreItems);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        GivenForbiddenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetStoreItems);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserTriesToQuery(ApiResources.GetStoreItems);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenAccountsExists()
    {
        const string firstCode = nameof(firstCode);
        const string secondCode = nameof(secondCode);

        await GivenAStoreItem()
            .ForUser(AuthorizedUserId)
            .WithCode(firstCode)
            .SavedInDb(_objectStoreDatabaseDriver);

        await GivenAStoreItem()
            .ForUser(AuthorizedUserId)
            .WithCode(secondCode)
            .SavedInDb(_objectStoreDatabaseDriver);

        await GivenAStoreItem()
            .SavedInDb(_objectStoreDatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetStoreItems, sorts: "-code");

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<StoreItemViewModel[]>();
        ThenAssertAll(() =>
        {
            result.Should().NotBeNull().And.HaveCount(2);
            result![0].Code.Should().Be(secondCode);
            result[1].Code.Should().Be(firstCode);
        });
    }
}
