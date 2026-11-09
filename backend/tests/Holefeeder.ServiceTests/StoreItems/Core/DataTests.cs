using Holefeeder.Service.StoreItems.Core;

namespace Holefeeder.ServiceTests.StoreItems.Core;

public class DataTests
{
    [Fact]
    public void Create_Succeeds_WhenValueHasContent()
    {
        var data = Data.Create("{\"theme\":\"dark\"}");

        data.Should().BeSuccessful().And.WithValue(Data.Valid("{\"theme\":\"dark\"}"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Fails_WhenValueIsEmpty(string? value)
    {
        var data = Data.Create(value);

        data.Should().BeFailure().And.WithError(DataErrors.Invalid);
    }

    [Fact]
    public void Invalid_DescribesTheEmptyRule()
    {
        DataErrors.Invalid.Code.Should().Be("data-invalid");
        DataErrors.Invalid.Description.Should().Be("Data cannot be empty.");
    }
}
