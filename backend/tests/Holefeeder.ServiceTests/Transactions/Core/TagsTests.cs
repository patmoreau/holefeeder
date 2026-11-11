using Holefeeder.Service.Transactions.Core;

namespace Holefeeder.ServiceTests.Transactions.Core;

public class TagsTests
{
    [Fact]
    public void Create_LowercasesTrimsAndDeduplicates() =>
        Tags.Create(["Weekly", " costco ", "weekly", "", "  ", null]).Values.Should().Equal("weekly", "costco");

    [Fact]
    public void Create_IsEmpty_WhenThereAreNoTags() =>
        Tags.Create(null).Values.Should().BeEmpty();

    [Fact]
    public void Text_JoinsTheTagsWithCommas() =>
        Tags.Create(["weekly", "costco"]).Text.Should().Be("weekly,costco");
}
