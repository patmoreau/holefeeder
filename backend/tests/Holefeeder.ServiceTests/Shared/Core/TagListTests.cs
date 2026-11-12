using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Shared.Core;

public class TagListTests
{
    [Fact]
    public void Create_LowercasesTrimsAndDeduplicates() =>
        TagList.Create(["Weekly", " costco ", "weekly", "", "  ", null]).Values.Should().Equal("weekly", "costco");

    [Fact]
    public void Create_IsEmpty_WhenThereAreNoTags() =>
        TagList.Create(null).Values.Should().BeEmpty();

    [Fact]
    public void Text_JoinsTheTagsWithCommas() =>
        TagList.Create(["weekly", "costco"]).Text.Should().Be("weekly,costco");
}
