using Holefeeder.Service.Tags.Core;

namespace Holefeeder.ServiceTests.Tags.Core;

public class TagTextTests
{
    [Theory]
    [InlineData("home,car,rent", "car", "auto", "home,auto,rent")]
    [InlineData("carwash,car", "car", "auto", "carwash,auto")]
    [InlineData("car,auto,home", "car", "auto", "auto,home")]
    [InlineData("auto,home,car", "car", "auto", "auto,home")]
    [InlineData("home, car ,rent", "car", "auto", "home,auto,rent")]
    [InlineData("Car,car", "Car", "car", "car")]
    [InlineData("home,rent", "car", "auto", "home,rent")]
    public void Rename_ReplacesTheExactTagInPlaceWithoutDuplicates(string text, string tag, string newTag, string expected) =>
        TagText.Rename(text, TagName.Valid(tag), TagName.Valid(newTag)).Should().Be(expected);

    [Theory]
    [InlineData("home,car,rent", "car", "home,rent")]
    [InlineData("carwash,car", "car", "carwash")]
    [InlineData("home, car ", "car", "home")]
    [InlineData("car", "car", "")]
    [InlineData("Car,car", "Car", "car")]
    [InlineData("home,rent", "car", "home,rent")]
    public void Remove_DropsTheExactTagAndKeepsTheOthersInPlace(string text, string tag, string expected) =>
        TagText.Remove(text, TagName.Valid(tag)).Should().Be(expected);
}
