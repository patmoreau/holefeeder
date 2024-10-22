using DrifterApps.Seeds.FluentResult;
using DrifterApps.Seeds.Testing;
using DrifterApps.Seeds.Testing.Attributes;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.Tests.Common.Builders;
using Holefeeder.Tests.Common.Extensions;
using Holefeeder.UnitTests.Domain.Extensions;

namespace Holefeeder.UnitTests.Domain.Features.Transactions;

[UnitTest, Category("Domain")]
public class TransactionTests
{
    private readonly Driver _driver = new();

    [Fact]
    public void GivenCreate_WhenDateIsMissing_ThenReturnFailure()
    {
        // arrange
        var driver = _driver.WithNoDate();

        // act
        var result = driver.Build();

        // assert
        result.ShouldHaveError(TransactionErrors.DateRequired);
    }

    [Fact]
    public void GivenCreate_WhenAccountIdIsMissing_ThenReturnFailure()
    {
        // arrange
        var driver = _driver.WithNoAccount();

        // act
        var result = driver.Build();

        // assert
        result.ShouldHaveError(TransactionErrors.AccountIdRequired);
    }

    [Fact]
    public void GivenCreate_WhenCategoryIdIsMissing_ThenReturnFailure()
    {
        // arrange
        var driver = _driver.WithNoCategory();

        // act
        var result = driver.Build();

        // assert
        result.ShouldHaveError(TransactionErrors.CategoryIdRequired);
    }

    [Fact]
    public void GivenCreate_WhenUserIdIsMissing_ThenReturnFailure()
    {
        // arrange
        var driver = _driver.WithNoUser();

        // act
        var result = driver.Build();

        // assert
        result.ShouldHaveError(TransactionErrors.UserIdRequired);
    }

    [Fact]
    public void GivenCreate_WhenValid_ThenReturnSuccess()
    {
        // arrange

        // act
        var result = _driver.Build();

        // assert
        result.Should().BeSuccessful();
        _driver.ShouldBeValid(result.Value);
    }

    [Fact]
    public void GivenImport_WhenIdIsMissing_ThenReturnFailure()
    {
        // arrange
        var driver = _driver.WithNoId();

        // act
        var result = driver.BuildWithImport();

        // assert
        result.ShouldHaveError(TransactionErrors.IdRequired);
    }

    [Fact]
    public void GivenImport_WhenDateIsMissing_ThenReturnFailure()
    {
        // arrange
        var driver = _driver.WithNoDate();

        // act
        var result = driver.BuildWithImport();

        // assert
        result.ShouldHaveError(TransactionErrors.DateRequired);
    }

    [Fact]
    public void GivenImport_WhenAccountIdIsMissing_ThenReturnFailure()
    {
        // arrange
        var driver = _driver.WithNoAccount();

        // act
        var result = driver.BuildWithImport();

        // assert
        result.ShouldHaveError(TransactionErrors.AccountIdRequired);
    }

    [Fact]
    public void GivenImport_WhenCategoryIdIsMissing_ThenReturnFailure()
    {
        // arrange
        var driver = _driver.WithNoCategory();

        // act
        var result = driver.BuildWithImport();

        // assert
        result.ShouldHaveError(TransactionErrors.CategoryIdRequired);
    }

    [Fact]
    public void GivenImport_WhenUserIdIsMissing_ThenReturnFailure()
    {
        // arrange
        var driver = _driver.WithNoUser();

        // act
        var result = driver.BuildWithImport();

        // assert
        result.ShouldHaveError(TransactionErrors.UserIdRequired);
    }

    [Fact]
    public void GivenImport_WhenValid_ThenReturnSuccess()
    {
        // arrange

        // act
        var result = _driver.BuildWithImport();

        // assert
        result.Should().BeSuccessful();
        _driver.ShouldBeValidWithImport(result.Value);
    }

    [Fact]
    public void GivenSetTags_WhenEmptyList_ThenTagListIsEmpty()
    {
        // arrange
        var transaction = _driver.Build().Value;

        // act
        var result = transaction.SetTags();

        // assert
        result.Should().BeSuccessful();
        result.Value.Tags.Should().BeEmpty();
    }

    [Fact]
    public void GivenSetTags_WhenAddingTags_ThenTagListIsSet()
    {
        // arrange
        var transaction = _driver.Build().Value;
        string[]? newTags = Fakerizer.Random.WordsArray(RandomCollectionCount());

        // act
        var result = transaction.SetTags(newTags);

        // assert
        result.Should().BeSuccessful();
        result.Value.Tags.Should().Contain(newTags.Select(x => x.ToLowerInvariant()));
    }

    [Fact]
    public void GivenApplyCashflow_WhenCashflowIdEmpty_ThenReturnFailure()
    {
        // arrange
        var transaction = _driver.Build().Value;

        // act
        var result = transaction.ApplyCashflow(CashflowId.Empty, Fakerizer.Date.RecentDateOnly());

        // assert
        result.ShouldHaveError(TransactionErrors.CashflowRequired);
    }

    [Fact]
    public void GivenApplyCashflow_WhenDateIsMissing_ThenReturnFailure()
    {
        // arrange
        var transaction = _driver.Build().Value;

        // act
        var result = transaction.ApplyCashflow(CashflowId.New, default);

        // assert
        result.ShouldHaveError(TransactionErrors.CashflowRequired);
    }

    [Fact]
    public void GivenApplyCashflow_WhenValid_ThenSetTransaction()
    {
        // arrange
        var transaction = _driver.Build().Value;
        var cashflowId = (CashflowId)Fakerizer.RandomGuid();
        var cashflowDate = Fakerizer.Date.RecentDateOnly();

        // act
        var result = transaction.ApplyCashflow(cashflowId, cashflowDate);

        // assert
        using (new AssertionScope())
        {
            result.Should().BeSuccessful();
            result.Value.CashflowId.Should().Be(cashflowId);
            result.Value.CashflowDate.Should().Be(cashflowDate);
        }
    }

    private sealed class Driver : IDriverOf<Result<Transaction>>
    {
        private static readonly Faker Faker = new();
        private TransactionId _id = TransactionId.New;
        private DateOnly _date = Faker.Date.RecentDateOnly();
        private readonly Money _amount = MoneyBuilder.Create().Build();
        private readonly string _description = Faker.Lorem.Sentence();
        private AccountId _accountId = (AccountId)Faker.Random.Guid();
        private CategoryId _categoryId = (CategoryId)Faker.Random.Guid();
        private UserId _userId = (UserId)Faker.Random.Guid();

        public Result<Transaction> Build() =>
            Transaction.Create(_date,
                _amount,
                _description,
                _accountId,
                _categoryId,
                _userId);

        public Result<Transaction> BuildWithImport() =>
            Transaction.Import(_id,
                _date,
                _amount,
                _description,
                _accountId,
                _categoryId,
                null,
                null,
                _userId);

        public Driver WithNoId()
        {
            _id = TransactionId.Empty;
            return this;
        }

        public Driver WithNoDate()
        {
            _date = default;
            return this;
        }

        public Driver WithNoAccount()
        {
            _accountId = AccountId.Empty;
            return this;
        }

        public Driver WithNoCategory()
        {
            _categoryId = CategoryId.Empty;
            return this;
        }

        public Driver WithNoUser()
        {
            _userId = UserId.Empty;
            return this;
        }

        public void ShouldBeValid(Transaction value)
        {
            using var scope = new AssertionScope();
            value.Id.Should().NotBe(TransactionId.Empty);
            value.Date.Should().Be(_date);
            value.Amount.Should().Be(_amount);
            value.Description.Should().Be(_description);
            value.AccountId.Should().Be(_accountId);
            value.CategoryId.Should().Be(_categoryId);
            value.CashflowId.Should().BeNull();
            value.CashflowDate.Should().BeNull();
            value.UserId.Should().Be(_userId);
        }

        public void ShouldBeValidWithImport(Transaction value)
        {
            using var scope = new AssertionScope();
            value.Id.Should().Be(_id);
            value.Date.Should().Be(_date);
            value.Amount.Should().Be(_amount);
            value.Description.Should().Be(_description);
            value.AccountId.Should().Be(_accountId);
            value.CategoryId.Should().Be(_categoryId);
            value.CashflowId.Should().BeNull();
            value.CashflowDate.Should().BeNull();
            value.UserId.Should().Be(_userId);
        }
    }
}
