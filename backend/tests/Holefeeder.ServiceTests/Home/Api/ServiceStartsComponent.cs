using System.Net;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

namespace Holefeeder.ServiceTests.Home.Api;

[Collection(ComponentTestMarker.Name)]
public class ServiceStartsComponent(ServerForTest serverForTest, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    [Fact]
    public Task WhenTheServiceStartsAgainstAnEmptyDatabase() =>
        ScenarioRunner.Create("The service starts against an empty database", _scenarioOutput)
            .Given("an empty database", () => { })
            .When("the service is asked for its health", AskingForHealth)
            .Then<HttpResponseMessage>("it answers healthy", TheServiceIsHealthy)
            .PlayAsync();

    private async Task<HttpResponseMessage> AskingForHealth()
    {
        using var client = serverForTest.CreateClient();
        return await client.GetAsync(new Uri("/healthz", UriKind.Relative));
    }

    private static async Task TheServiceIsHealthy(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        using var message = response.Value;
        message.StatusCode.Should().Be(HttpStatusCode.OK);
        (await message.Content.ReadAsStringAsync()).Should().Be("Healthy");
    }
}
