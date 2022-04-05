using System;
using System.Threading.Tasks;

using AutoBogus;

using FluentAssertions;
using FluentAssertions.Execution;

using Holefeeder.Domain.Features.StoreItem;

using Xunit;

namespace Holefeeder.UnitTests.Domain.Features.StoreItem;

public class StoreItemTests
{
    [Fact]
    public async Task GivenStoreItem_WhenIdIsNull_ThenThrowException()
    {
        // arrange
        var id = Guid.Empty;
        var code = AutoFaker.Generate<string>();
        var userId = AutoFaker.Generate<Guid>();

        // act
        Func<Task> action = () => Task.FromResult(new Holefeeder.Domain.Features.StoreItem.StoreItem(id, code, userId));

        // assert
        await action.Should().ThrowAsync<ObjectStoreDomainException>().WithMessage("'Id' is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public async Task GivenStoreItem_WhenCodeIsNull_ThenThrowException(string code)
    {
        // arrange
        var id = AutoFaker.Generate<Guid>();
        var userId = AutoFaker.Generate<Guid>();

        // act
        Func<Task> action = () => Task.FromResult(new Holefeeder.Domain.Features.StoreItem.StoreItem(id, code, userId));

        // assert
        await action.Should().ThrowAsync<ObjectStoreDomainException>().WithMessage("'Code' is required");
    }

    [Fact]
    public async Task GivenStoreItem_WhenUserIdIsNull_ThenThrowException()
    {
        // arrange
        var id = AutoFaker.Generate<Guid>();
        var code = AutoFaker.Generate<string>();
        var userId = Guid.Empty;

        // act
        Func<Task> action = () => Task.FromResult(new Holefeeder.Domain.Features.StoreItem.StoreItem(id, code, userId));

        // assert
        await action.Should().ThrowAsync<ObjectStoreDomainException>().WithMessage("'UserId' is required");
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
        Holefeeder.Domain.Features.StoreItem.StoreItem item = null!;
        Func<Task> action = () =>
        {
            item = new Holefeeder.Domain.Features.StoreItem.StoreItem(id, code, userId)
            {
                Data = data
            };
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
