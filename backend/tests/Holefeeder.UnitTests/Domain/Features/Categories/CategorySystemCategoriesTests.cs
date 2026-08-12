using DrifterApps.Seeds.FluentResult;
using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.UnitTests.Domain.Extensions;

namespace Holefeeder.UnitTests.Domain.Features.Categories;

[UnitTest]
public class CategorySystemCategoriesTests
{
    private readonly Driver _driver = new();

    [Fact]
    public void GivenCreateSystemCategories_WhenValid_ThenTheTransferCategoriesAreCreated()
    {
        // arrange

        // act
        var result = _driver.Build();

        // assert
        using var scope = new AssertionScope();
        result.Should().BeSuccessful();

        var transferOut = result.Value.Should().ContainSingle(category => category.Name == Category.TransferOutName)
            .Subject;
        transferOut.Type.Should().Be(CategoryType.Expense);

        var transferIn = result.Value.Should().ContainSingle(category => category.Name == Category.TransferInName)
            .Subject;
        transferIn.Type.Should().Be(CategoryType.Gain);
    }

    [Fact]
    public void GivenCreateSystemCategories_WhenValid_ThenTheyBelongToTheUserAndAreFlaggedSystem()
    {
        // arrange

        // act
        var categories = _driver.Build().Value;

        // assert
        using var scope = new AssertionScope();
        categories.Should().HaveCount(2).And.AllSatisfy(category =>
        {
            category.UserId.Should().Be(Driver.OwnerId);
            category.System.Should().BeTrue();
            category.Favorite.Should().BeFalse();
            category.Inactive.Should().BeFalse();
            category.BudgetAmount.Should().Be(Money.Zero);
        });
    }

    [Fact]
    public void GivenCreateSystemCategories_WhenUserIdIsInvalid_ThenReturnFailure()
    {
        // arrange
        var driver = _driver.WithEmptyUserId();

        // act
        var result = driver.Build();

        // assert
        result.ShouldHaveError(CategoryErrors.UserIdRequired);
    }

    private sealed class Driver : IDriverOf<Result<IReadOnlyCollection<Category>>>
    {
        public static readonly UserId OwnerId = (UserId)new Faker().Random.Guid();

        private UserId _userId = OwnerId;

        public Driver WithEmptyUserId()
        {
            _userId = UserId.Empty;
            return this;
        }

        public Result<IReadOnlyCollection<Category>> Build() => Category.CreateSystemCategories(_userId);
    }
}
