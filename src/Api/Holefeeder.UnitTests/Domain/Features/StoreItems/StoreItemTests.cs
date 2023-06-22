using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Tests.Common.Builders.StoreItems;

namespace Holefeeder.UnitTests.Domain.Features.StoreItems;

[UnitTest]
public class StoreItemTests
{
    [Fact]
    public void GivenStoreItem_WhenIdIsNull_ThenThrowException()
    {
        // arrange

        // act
        Action action = () => _ = StoreItemBuilder.GivenAStoreItem().WithNoId().Build();

        // assert
        action.Should()
            .Throw<ObjectStoreDomainException>()
            .WithMessage("'Id' is required")
            .And
            .Context.Should().Be(nameof(StoreItem));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void GivenStoreItem_WhenCodeIsNull_ThenThrowException(string code)
    {
        // arrange

        // act
        Action action = () => _ = StoreItemBuilder.GivenAStoreItem().WithCode(code).Build();

        // assert
        action.Should()
            .Throw<ObjectStoreDomainException>()
            .WithMessage("'Code' is required")
            .And
            .Context.Should().Be(nameof(StoreItem));
    }

    [Fact]
    public void GivenStoreItem_WhenUserIdIsNull_ThenThrowException()
    {
        // arrange

        // act
        Action action = () => _ = StoreItemBuilder.GivenAStoreItem().ForNoUser().Build();

        // assert
        action.Should()
            .Throw<ObjectStoreDomainException>()
            .WithMessage("'UserId' is required")
            .And
            .Context.Should().Be(nameof(StoreItem));
    }

    [Fact]
    public void GivenStoreItem_WhenValid_ThenValidEntity()
    {
        // arrange

        // act
        var item = StoreItemBuilder.GivenAStoreItem().Build();

        // assert
        item.Should().NotBeNull();
    }
}
