using System.Threading.Tasks;
using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.UnitTests.Domain.Features.StoreItems;

public class StoreItemTests
{
    [Fact]
    public void GivenStoreItem_WhenIdIsNull_ThenThrowException()
    {
        // arrange
        Guid id = Guid.Empty;
        string? code = AutoFaker.Generate<string>();
        Guid userId = AutoFaker.Generate<Guid>();

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
        Guid id = AutoFaker.Generate<Guid>();
        Guid userId = AutoFaker.Generate<Guid>();

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
        Guid id = AutoFaker.Generate<Guid>();
        string? code = AutoFaker.Generate<string>();
        Guid userId = Guid.Empty;

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
        Guid id = AutoFaker.Generate<Guid>();
        string? code = AutoFaker.Generate<string>();
        string? data = AutoFaker.Generate<string>();
        Guid userId = AutoFaker.Generate<Guid>();

        // act
        StoreItem item = null!;
        Func<Task> action = () =>
        {
            item = new StoreItem(id, code, userId) { Data = data };
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
