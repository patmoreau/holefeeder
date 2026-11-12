using Holefeeder.Service.Tags.Core;

namespace Holefeeder.ServiceTests.Tags.Core;

public class TagNameTests
{
    [Theory]
    [InlineData("car", "car")]
    [InlineData("  Car Wash ", "car wash")]
    [InlineData("ÉTÉ", "été")]
    public void Create_TrimsAndLowercasesTheName(string value, string expected) =>
        TagName.Create(value).Should().BeSuccessful().And.WithValue(TagName.Valid(expected));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("car,wash")]
    public void Create_Fails_WhenTheNameIsEmptyOrHoldsAComma(string? value) =>
        TagName.Create(value).Should().BeFailure().And.WithError(TagNameErrors.Invalid);

    [Theory]
    [InlineData("Car", "Car")]
    [InlineData("  car ", "car")]
    public void Existing_OnlyTrimsTheName(string value, string expected) =>
        TagName.Existing(value).Should().BeSuccessful().And.WithValue(TagName.Valid(expected));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Existing_Fails_WhenTheNameIsEmpty(string? value) =>
        TagName.Existing(value).Should().BeFailure().And.WithError(TagNameErrors.Required);

    [Fact]
    public void Errors_FollowTheServiceConventions()
    {
        TagNameErrors.Invalid.Code.Should().Be("tag-name-invalid");
        TagNameErrors.Invalid.Description.Should().Be("A tag name must not be empty or contain a comma.");
        TagNameErrors.Required.Code.Should().Be("tag-required");
        TagNameErrors.Required.Description.Should().Be("A tag is required.");
    }
}
