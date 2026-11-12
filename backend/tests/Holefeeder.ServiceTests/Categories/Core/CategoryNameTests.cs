using Holefeeder.Service.Categories.Core;

namespace Holefeeder.ServiceTests.Categories.Core;

public class CategoryNameTests
{
    [Fact]
    public void Create_KeepsTheTrimmedName() =>
        CategoryName.Create("  Groceries ").Should().BeSuccessful().And.WithValue(CategoryName.Valid("Groceries"));

    [Fact]
    public void Create_AcceptsTwoHundredFiftyFiveCharacters() =>
        CategoryName.Create(new string('a', 255)).Should().BeSuccessful();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Fails_WhenTheNameIsEmpty(string? value) =>
        CategoryName.Create(value).Should().BeFailure().And.WithError(CategoryNameErrors.Invalid);

    [Fact]
    public void Create_Fails_WhenTheNameIsTooLong() =>
        CategoryName.Create(new string('a', 256)).Should().BeFailure().And.WithError(CategoryNameErrors.Invalid);

    [Fact]
    public void Invalid_DescribesTheLengthRule()
    {
        CategoryNameErrors.Invalid.Code.Should().Be("category-name-invalid");
        CategoryNameErrors.Invalid.Description.Should().Be("Category name must be between 1 and 255 characters.");
    }
}
