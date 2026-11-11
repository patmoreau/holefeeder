using System.Text.Json;
using System.Text.Json.Serialization;

using Holefeeder.Service.Accounts.Api;

namespace Holefeeder.ServiceTests.Accounts.Api;

public class JsonScalarAsTextTests
{
    [Theory]
    [InlineData("""{"value":"250.75"}""", "250.75")]
    [InlineData("""{"value":250.75}""", "250.75")]
    [InlineData("""{"value":-1}""", "-1")]
    [InlineData("""{"value":true}""", "true")]
    [InlineData("""{"value":false}""", "false")]
    [InlineData("""{"value":null}""", null)]
    [InlineData("""{"value":{"amount":1}}""", null)]
    [InlineData("""{"value":[1]}""", null)]
    [InlineData("{}", null)]
    public void Read_KeepsTheScalarAsText(string json, string? expected) =>
        JsonSerializer.Deserialize<Body>(json, JsonSerializerOptions.Web)!.Value.Should().Be(expected);

    private sealed record Body([property: JsonConverter(typeof(JsonScalarAsText))] string? Value);
}
