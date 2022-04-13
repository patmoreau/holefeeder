using System.Net;

using FluentAssertions;

using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using Microsoft.AspNetCore.Mvc;

using Xunit;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

public class GetStoreItemsTests : BaseFeatureTests, IClassFixture<HttpClientDriver>
{
    public GetStoreItemsTests(HttpClientDriver httpClientDriver) : base(httpClientDriver)
    {
    }

    [Fact]
    public async Task Get_Authorized()
    {
        // Given I am authorized
        await GivenAuthorizationSet(true);

        // When I try to fetch StoreItems
        await HttpClientDriver.SendGetRequest(ApiResources.StoreItems);

        // Then I should be authorized to access the endpoint
        ThenShouldHaveProperAuthorization(true);
    }

    [Fact]
    public async Task Get_Unauthorized()
    {
        // Given I am authorized
        await GivenAuthorizationSet(false);

        // When I try to fetch StoreItems
        await HttpClientDriver.SendGetRequest(ApiResources.StoreItems);

        // Then I should not be authorized to access the endpoint
        ThenShouldHaveProperAuthorization(false);
    }

    [Fact]
    public async Task Get_Happy_Path()
    {
        // Given I am authorized
        await GivenAuthorizationSet(false);

        // When I try to fetch StoreItems
        await HttpClientDriver.SendGetRequest(ApiResources.StoreItems);

        // Then I expect a '200' status code
        HttpClientDriver.ShouldHaveResponseWithStatus(HttpStatusCode.OK);

        // And I get my expected items
        var result = await HttpClientDriver.DeserializeContent<StoreItemViewModel[]>();

        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("0", "-1", "data", "code:eq:settings")]
    public async Task Get_Invalid_Request(string offset, string limit, string sort, string filter)
    {
        // Given I am authorized
        await GivenAuthorizationSet(true);

        // When I try to fetch StoreItemsQuery using query params with offset '0' and limit '-1' and sort 'date' and filter 'code:eq:settings'
        await HttpClientDriver.SendGetRequest(ApiResources.StoreItemsQuery, offset, limit, sort, filter);

        // Then I expect a '422' status code
        HttpClientDriver.ShouldHaveResponseWithStatus(HttpStatusCode.UnprocessableEntity);

        // And I get an problem details with error message saying 'One or more validation errors occurred.'
        var result = await HttpClientDriver.DeserializeContent<ProblemDetails>();
        result.Should().NotBeNull();
        result?.Title.Should().Be("One or more validation errors occurred.");
    }
}
