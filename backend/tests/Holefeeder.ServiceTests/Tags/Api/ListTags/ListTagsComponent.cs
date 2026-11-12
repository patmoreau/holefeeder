using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Tags.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

namespace Holefeeder.ServiceTests.Tags.Api.ListTags;

[Collection(ComponentTestMarker.Name)]
public class ListTagsComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;

    [Fact]
    public Task WhenTheUserHasTags() =>
        ScenarioRunner.Create("Listing the tags to manage", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has tagged transactions and cashflows", TheUserHasTags)
            .When("the user lists the tags", ListingTheTags)
            .Then<HttpResponseMessage>("the tags are listed with their usage", TheTagsAreListed)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Listing the tags as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .When("the user lists the tags", ListingTheTags)
            .Then<HttpResponseMessage>("nothing is listed", NothingIsListed)
            .PlayAsync();

    private async Task ARegisteredUser() => _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasTags()
    {
        await server.Database.TaggedTransactionInDbAsync(_user, "groceries");
        await server.Database.TaggedTransactionInDbAsync(_user, "groceries,home");
        await server.Database.TaggedCashflowInDbAsync(_user, "home");
    }

    private async Task<HttpResponseMessage> ListingTheTags()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/tags/manage", UriKind.Relative));
    }

    private static async Task TheTagsAreListed(Ensure<HttpResponseMessage> response) =>
        (await TheTagsAsync(response)).Should().Equal(new TagJson("groceries", 2, 0), new TagJson("home", 1, 1));

    private static async Task NothingIsListed(Ensure<HttpResponseMessage> response) =>
        (await TheTagsAsync(response)).Should().BeEmpty();

    private static async Task<List<TagJson>> TheTagsAsync(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await response.Value.Content.ReadFromJsonAsync<List<TagJson>>())!;
    }

    private sealed record TagJson(string Tag, int TransactionCount, int ActiveCashflowCount);
}
