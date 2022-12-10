using System;
using System.Linq;

using Bogus;
using Bogus.Extensions;

using FluentAssertions;

using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.UnitTests.Domain.Features.Categories;

public class CategoryTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void GivenConstructor_WhenIdIsEmpty_ThenThrowException()
    {
        // arrange

        // act
        Action action = () => _ = new Category(Guid.Empty,
            _faker.PickRandom(CategoryType.List.ToArray()),
            _faker.Random.Word(),
            _faker.Random.Guid());

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
        Action action = () => _ = new Category(_faker.Random.Guid(),
            _faker.PickRandom(CategoryType.List.ToArray()),
            name,
            _faker.Random.Guid());

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
        Action action = () => _ = new Category(_faker.Random.Guid(),
            _faker.PickRandom(CategoryType.List.ToArray()),
            _faker.Random.Words().ClampLength(256),
            _faker.Random.Guid());

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
        Action action = () => _ = new Category(_faker.Random.Guid(),
            _faker.PickRandom(CategoryType.List.ToArray()),
            _faker.Random.Word(),
            Guid.Empty);

        // assert
        action.Should().Throw<CategoryDomainException>()
            .WithMessage("UserId is required")
            .And
            .Context.Should().Be(nameof(Category));
    }
}
