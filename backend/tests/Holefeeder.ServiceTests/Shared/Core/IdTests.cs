using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Shared.Core;

public class IdTests
{
    [Fact]
    public void Create_Succeeds_WhenValid()
    {
        var value = Guid.NewGuid();
        var result = Id.Create(value);

        result.Should().BeSuccessful().And.WithValue(Id.Valid(value));
    }

    [Fact]
    public void Create_Fails_WhenValueIsInvalid()
    {
        var value = Guid.Empty;
        var result = Id.Create(value);

        result.Should().BeFailure().And.WithError(IdErrors.Invalid);
    }

    [Fact]
    public void New_ReturnsADistinctNonEmptyId()
    {
        var first = Id.New();
        var second = Id.New();

        first.Should().NotBe(Id.Empty);
        second.Should().NotBe(first);
    }

    [Fact]
    public void Create_Succeeds_WhenTextIsAGuid()
    {
        var value = Guid.NewGuid();
        var result = Id.Create(value.ToString());

        result.Should().BeSuccessful().And.WithValue(Id.Valid(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-guid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void Create_Fails_WhenTextIsNotAnId(string? value)
    {
        var result = Id.Create(value);

        result.Should().BeFailure().And.WithError(IdErrors.Invalid);
    }
}
