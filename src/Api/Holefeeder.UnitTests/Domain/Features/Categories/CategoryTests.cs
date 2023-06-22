using Holefeeder.Domain.Features.Categories;
using Holefeeder.Tests.Common.Builders.Categories;

namespace Holefeeder.UnitTests.Domain.Features.Categories;

[UnitTest]
public class CategoryTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void GivenConstructor_WhenIdIsEmpty_ThenThrowException()
    {
        // arrange

        // act
        Action action = () => _ = CategoryBuilder.GivenACategory().WithNoId().Build();

        // assert
        action.Should().Throw<CategoryDomainException>()
            .WithMessage("Id is required")
            .And
            .Context.Should().Be(nameof(Category));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenConstructor_WhenNameIsEmpty_ThenThrowException(string name)
    {
        // arrange

        // act
        Action action = () => _ = CategoryBuilder.GivenACategory().WithName(name).Build();

        // assert
        action.Should().Throw<CategoryDomainException>()
            .WithMessage("Name must be from 1 to 255 characters")
            .And
            .Context.Should().Be(nameof(Category));
    }

    [Fact]
    public void GivenConstructor_WhenNameIsTooLong_ThenThrowException()
    {
        // arrange

        // act
        Action action = () =>
            _ = CategoryBuilder.GivenACategory().WithName(_faker.Random.Words().ClampLength(256)).Build();

        // assert
        action.Should().Throw<CategoryDomainException>()
            .WithMessage("Name must be from 1 to 255 characters")
            .And
            .Context.Should().Be(nameof(Category));
    }

    [Fact]
    public void GivenConstructor_WhenUserIdIsEmpty_ThenThrowException()
    {
        // arrange

        // act
        Action action = () => _ = CategoryBuilder.GivenACategory().ForNoUser().Build();

        // assert
        action.Should().Throw<CategoryDomainException>()
            .WithMessage("UserId is required")
            .And
            .Context.Should().Be(nameof(Category));
    }
}
