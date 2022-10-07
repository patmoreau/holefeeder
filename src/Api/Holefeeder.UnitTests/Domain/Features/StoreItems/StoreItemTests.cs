using System;
using System.Threading.Tasks;

using AutoBogus;

using FluentAssertions;
using FluentAssertions.Execution;

using Holefeeder.Domain.Features.StoreItem;

using Xunit;

namespace Holefeeder.UnitTests.Domain.Features.StoreItems;

public class StoreItemTests
{
    [Fact]
    public void GivenStoreItem_WhenIdIsNull_ThenThrowException()
    {
        // arrange
        var id = Guid.Empty;
        var code = AutoFaker.Generate<string>();
        var userId = AutoFaker.Generate<Guid>();

        // act
        Action action = () => _ = new StoreItem(id, code, userId);

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
        var id = AutoFaker.Generate<Guid>();
        var userId = AutoFaker.Generate<Guid>();

        // act
        Action action = () => _ = new StoreItem(id, code, userId);

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
        var id = AutoFaker.Generate<Guid>();
        var code = AutoFaker.Generate<string>();
        var userId = Guid.Empty;

        // act
        Action action = () => _ = new StoreItem(id, code, userId);

        // assert
        action.Should()
            .Throw<ObjectStoreDomainException>()
            .WithMessage("'UserId' is required")
            .And
            .Context.Should().Be(nameof(StoreItem));
    }

    [Fact]
    public async Task GivenStoreItem_WhenValid_ThenValidEntity()
    {
        // arrange
        var id = AutoFaker.Generate<Guid>();
        var code = AutoFaker.Generate<string>();
        var data = AutoFaker.Generate<string>();
        var userId = AutoFaker.Generate<Guid>();

        // act
        StoreItem item = null!;
        var action = () =>
        {
            item = new StoreItem(id, code, userId) {Data = data};
            return Task.CompletedTask;
        };

        // assert
        using (new AssertionScope())
        {
            await action.Should().NotThrowAsync();
            item.Id.Should().Be(id);
            item.Code.Should().Be(code);
            item.Data.Should().Be(data);
            item.UserId.Should().Be(userId);
        }
    }
}
