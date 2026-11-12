using Holefeeder.Service.Categories.Core;

namespace Holefeeder.ServiceTests.Categories.Core;

public class CategoryColorTests
{
    [Theory]
    [InlineData("#34C759")]
    [InlineData("#a2845e")]
    public void Create_AcceptsAHexColor(string value) =>
        CategoryColor.Create(value).Should().BeSuccessful().And.WithValue(CategoryColor.Valid(value));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("34C759")]
    [InlineData("#34C75")]
    [InlineData("#34C7590")]
    [InlineData("#GGGGGG")]
    [InlineData("green")]
    public void Create_Fails_WhenTheColorIsNotAHexColor(string? value) =>
        CategoryColor.Create(value).Should().BeFailure().And.WithError(CategoryColorErrors.Invalid);

    [Fact]
    public void Invalid_DescribesTheFormat()
    {
        CategoryColorErrors.Invalid.Code.Should().Be("category-color-invalid");
        CategoryColorErrors.Invalid.Description.Should().Be("Category color must be a hex color as #RRGGBB.");
    }
}
