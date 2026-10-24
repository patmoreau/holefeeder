using DrifterApps.Seeds.Testing;

using Holefeeder.Tests.Common.Builders.Accounts;
using Holefeeder.Tests.Common.Builders.Categories;

namespace Holefeeder.UnitTests.TestBuilders;

[UnitTest]
public class UniqueNameTests
{
    private const int BuilderCount = 500;

    [Fact]
    public void GivenSeparateAccountBuilders_WhenBuilt_ThenEveryNameIsUnique()
    {
        // act
        var names = Enumerable.Range(0, BuilderCount)
            .Select(_ => AccountBuilder.GivenAnActiveAccount().Build().Name)
            .ToList();

        // assert
        names.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void GivenSeparateCategoryBuilders_WhenBuilt_ThenEveryNameIsUnique()
    {
        // act
        var names = Enumerable.Range(0, BuilderCount)
            .Select(_ => CategoryBuilder.GivenACategory().Build().Name)
            .ToList();

        // assert
        names.Should().OnlyHaveUniqueItems();
    }
}
