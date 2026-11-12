using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Tags.Core;
using Holefeeder.ServiceTests.Tags.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.Tags.Api.RenameTag;

[Collection(ComponentTestMarker.Name)]
public class RenameTagComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Id _transaction = null!;
    private Id _cashflow = null!;

    [Fact]
    public Task WhenRenamingATag() =>
        ScenarioRunner.Create("Renaming a tag", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a tagged transaction and cashflow", TheUserHasTaggedItems)
            .When("the user renames the tag", () => Renaming("car", "Auto"))
            .Then<HttpResponseMessage>("no content is returned", NoContentIsReturned)
            .And("the tag is renamed everywhere", TheTagIsRenamed)
            .PlayAsync();

    [Fact]
    public Task WhenTheRequestIsInvalid() =>
        ScenarioRunner.Create("Renaming a tag with an invalid request", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a tagged transaction and cashflow", TheUserHasTaggedItems)
            .When("the user renames no tag to a name with a comma", () => Renaming(" ", "auto,moto"))
            .Then<HttpResponseMessage>("a validation problem is returned", AValidationProblemIsReturned)
            .And("the tags are unchanged", TheTagsAreUnchanged)
            .PlayAsync();

    private async Task ARegisteredUser() => _user = await AUser().SavedInDbAsync(server.Database);

    private async Task TheUserHasTaggedItems()
    {
        _transaction = await server.Database.TaggedTransactionInDbAsync(_user, "home,car");
        _cashflow = await server.Database.TaggedCashflowInDbAsync(_user, "car");
    }

    private async Task<HttpResponseMessage> Renaming(string tag, string newTag)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(new Uri("/api/v2/tags/rename", UriKind.Relative), new { Tag = tag, NewTag = newTag });
    }

    private static void NoContentIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task TheTagIsRenamed()
    {
        (await server.Database.TagsOfAsync("transactions", _transaction)).Should().Be("home,auto");
        (await server.Database.TagsOfAsync("cashflows", _cashflow)).Should().Be("auto");
    }

    private async Task TheTagsAreUnchanged()
    {
        (await server.Database.TagsOfAsync("transactions", _transaction)).Should().Be("home,car");
        (await server.Database.TagsOfAsync("cashflows", _cashflow)).Should().Be("car");
    }

    private static async Task AValidationProblemIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Errors.Should().ContainKeys(TagNameErrors.CodeRequired, TagNameErrors.CodeInvalid);
    }
}
