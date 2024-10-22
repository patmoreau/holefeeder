using DrifterApps.Seeds.FluentResult;
using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.Tests.Common.Builders;
using Holefeeder.UnitTests.Domain.Extensions;

namespace Holefeeder.UnitTests.Domain.Features.Categories;

[UnitTest, Category("Domain")]
public class CategoryTests
{
    private readonly Driver _driver = new();

    [Fact]
    public void GivenImport_WhenNoValidationError_ThenReturnSuccessWithObject()
    {
        // arrange

        // act
        var result = _driver.BuildWithImport();

        // assert
        using var scope = new AssertionScope();
        result.Should().BeSuccessful().And.WithValue(result.Value);
        _driver.ShouldBeValidCategory(result.Value);
    }

    [Fact]
    public void GivenImport_WhenIdIsInvalid_ThenReturnFailure()
    {
        // arrange
        var driver = _driver.WithEmptyId();

        // act
        var result = driver.BuildWithImport();

        // assert
        result.ShouldHaveError(CategoryErrors.IdRequired);
    }

    [Theory]
    [ClassData(typeof(NameValidationData))]
    public void GivenImport_WhenNameIsInvalid_ThenReturnFailure(string? name)
    {
        // arrange
        var driver = _driver.WithName(name!);

        // act
        var result = driver.BuildWithImport();


        // assert
        result.ShouldHaveError(CategoryErrors.NameRequired);
    }

    [Fact]
    public void GivenImport_WhenUserIdIsInvalid_ThenReturnFailure()
    {
        // arrange
        var driver = _driver.WithEmptyUserId();

        // act
        var result = driver.BuildWithImport();

        // assert
        result.ShouldHaveError(CategoryErrors.UserIdRequired);
    }

    [Fact]
    public void GivenCreate_WhenNoValidationError_ThenReturnSuccessWithObject()
    {
        // arrange

        // act
        var result = _driver.Build();

        // assert
        using var scope = new AssertionScope();
        result.Should().BeSuccessful().And.WithValue(result.Value);
        _driver.ShouldBeValidCategory(result.Value);
    }

    internal sealed class NameValidationData : TheoryData<string?>
    {
        public NameValidationData()
        {
            Add(null);
            Add(string.Empty);
            Add("       ");
            Add(new Faker().Random.Words().ClampLength(256));
        }
    }

    private sealed class Driver : IDriverOf<Result<Category>>
    {
        private static readonly Faker Faker = new();
        private CategoryId _categoryId = CategoryId.New;
        private readonly CategoryType _categoryType = Faker.PickRandom<CategoryType>(CategoryType.List);
        private string _name = Faker.Lorem.Word();
        private readonly CategoryColor _categoryColor = CategoryColorBuilder.Create().Build();
        private readonly bool _favorite = Faker.Random.Bool();
        private readonly bool _system = Faker.Random.Bool();
        private readonly Money _budgetAmount = MoneyBuilder.Create().Build();
        private UserId _userId = (UserId)Faker.Random.Guid();

        public Result<Category> Build() =>
            Category.Import(_categoryId,
                _categoryType,
                _name,
                _categoryColor,
                _favorite,
                _system,
                _budgetAmount,
                _userId);

        public Result<Category> BuildWithImport() =>
            Category.Import(_categoryId,
                _categoryType,
                _name,
                _categoryColor,
                _favorite,
                _system,
                _budgetAmount,
                _userId);

        public Driver WithEmptyId()
        {
            _categoryId = CategoryId.Empty;
            return this;
        }

        public Driver WithName(string name)
        {
            _name = name;
            return this;
        }

        public Driver WithEmptyUserId()
        {
            _userId = UserId.Empty;
            return this;
        }

        public void ShouldBeValidCategory(Category category)
        {
            using var scope = new AssertionScope();
            category.Id.Should().Be(_categoryId);
            category.Type.Should().Be(_categoryType);
            category.Name.Should().Be(_name);
            category.Color.Should().Be(_categoryColor);
            category.Favorite.Should().Be(_favorite);
            category.System.Should().Be(_system);
            category.BudgetAmount.Should().Be(_budgetAmount);
            category.UserId.Should().Be(_userId);
        }
    }
}
