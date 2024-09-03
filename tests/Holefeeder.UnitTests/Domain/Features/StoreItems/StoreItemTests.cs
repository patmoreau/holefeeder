using DrifterApps.Seeds.Domain;
using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.Features.Users;

namespace Holefeeder.UnitTests.Domain.Features.StoreItems;

[UnitTest, Category("Domain")]
public class StoreItemTests
{
    private readonly Driver _driver = new();

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void GivenCreate_WhenCodeIsInvalid_ThenReturnFailure(string? code)
    {
        // arrange
        var driver = _driver.WithCode(code!);

        // act
        var result = driver.Build();

        // assert
        result.Should().BeFailure()
            .WithError(ResultAggregateError.CreateValidationError([StoreItemErrors.CodeRequired]));
    }

    [Fact]
    public void GivenCreate_WhenUserIdIsInvalid_ThenReturnFailure()
    {
        // arrange
        var driver = _driver.WithEmptyUserId();

        // act
        var result = driver.Build();

        // assert
        result.Should().BeFailure()
            .WithError(ResultAggregateError.CreateValidationError([StoreItemErrors.UserIdRequired]));
    }

    [Fact]
    public void GivenCreate_WhenValid_ThenReturnSuccessWithObject()
    {
        // arrange

        // act
        var result = _driver.Build();

        // assert
        using var scope = new AssertionScope();
        result.Should().BeSuccessful().WithValue(result.Value);
        _driver.ShouldBeValidStoreItem(result.Value);
    }

    private sealed class Driver : IDriverOf<Result<StoreItem>>
    {
        private static readonly Faker Faker = new();
        private string _code = Faker.Random.Word();
        private readonly string _data = Faker.Random.Words();
        private UserId _userId = (UserId)Faker.Random.Guid();

        public Result<StoreItem> Build() => StoreItem.Create(_code, _data, _userId);

        public Driver WithCode(string code)
        {
            _code = code;
            return this;
        }

        public Driver WithEmptyUserId()
        {
            _userId = UserId.Empty;
            return this;
        }

        public void ShouldBeValidStoreItem(StoreItem storeItem)
        {
            using var scope = new AssertionScope();
            storeItem.Id.Should().NotBe(StoreItemId.Empty);
            storeItem.Code.Should().Be(_code);
            storeItem.Data.Should().Be(_data);
        }
    }
}
