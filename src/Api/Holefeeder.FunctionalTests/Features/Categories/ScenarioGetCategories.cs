using System.Net;

using FluentAssertions;

using Holefeeder.Application.Models;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using Xunit;
using Xunit.Abstractions;

using static Holefeeder.Tests.Common.Builders.CategoryEntityBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Categories;

public class ScenarioGetCategories : BaseScenario
{
    private readonly HolefeederDatabaseDriver _holefeederDatabaseDriver;

    public ScenarioGetCategories(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        _holefeederDatabaseDriver = apiApplicationDriver.CreateHolefeederDatabaseDriver();
        _holefeederDatabaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        GivenUserIsAuthorized();

        await WhenUserGetCategories();

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        GivenForbiddenUserIsAuthorized();

        await WhenUserGetCategories();

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserGetCategories();

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenCategoriesExists()
    {
        const string firstName = nameof(firstName);
        const string secondName = nameof(secondName);

        var firstCategory = await GivenACategory()
            .WithName(firstName)
            .ForUser(AuthorizedUserId)
            .SavedInDb(_holefeederDatabaseDriver);

        var secondCategory = await GivenACategory()
            .WithName(secondName)
            .ForUser(AuthorizedUserId)
            .SavedInDb(_holefeederDatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetCategories();

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<CategoryViewModel[]>();
        ThenAssertAll(() =>
        {
            result.Should().NotBeNull().And.HaveCount(2);
            result![0].Should().BeEquivalentTo(firstCategory,
                options => options.ExcludingMissingMembers());
            result[1].Should().BeEquivalentTo(secondCategory,
                options => options.ExcludingMissingMembers());
        });
    }

    private async Task WhenUserGetCategories()
    {
        await HttpClientDriver.SendGetRequest(ApiResources.GetCategories);
    }
}
