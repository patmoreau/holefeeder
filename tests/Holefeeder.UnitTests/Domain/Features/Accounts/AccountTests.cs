using DrifterApps.Seeds.FluentResult;
using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.Tests.Common.Builders;
using Holefeeder.Tests.Common.Builders.Accounts;
using Holefeeder.Tests.Common.Builders.Categories;
using Holefeeder.Tests.Common.Builders.Transactions;
using Holefeeder.UnitTests.Domain.Extensions;

namespace Holefeeder.UnitTests.Domain.Features.Accounts;

[UnitTest, Category("Domain")]
public class AccountTests
{
    private readonly Driver _driver = new();

    [Fact]
    public void GivenConstructor_WhenIdEmpty_ThenThrowException()
    {
        // arrange
        var driver = _driver.WithEmptyId();

        // act
        var result = driver.BuildWithImport();

        // assert
        result.ShouldHaveError(AccountErrors.IdRequired);
    }

    [Theory]
    [ClassData(typeof(NameValidationData))]
    public void GivenConstructor_WhenNameIsEmpty_ThenThrowException(string? name)
    {
        // arrange
        var driver = _driver.WithName(name!);

        // act
        var result = driver.Build();

        // assert
        result.ShouldHaveError(AccountErrors.NameRequired);
    }

    [Fact]
    public void GivenConstructor_WhenOpenDateIsMissing_ThenThrowException()
    {
        // arrange
        var driver = _driver.WithOpenDate(default);

        // act
        var result = driver.Build();

        // assert
        result.ShouldHaveError(AccountErrors.OpenDateRequired);
    }

    [Fact]
    public void GivenConstructor_WhenUserIdEmpty_ThenThrowException()
    {
        // arrange
        var driver = _driver.WithEmptyUserId();

        // act
        var result = driver.BuildWithImport();

        // assert
        result.ShouldHaveError(AccountErrors.UserIdRequired);
    }

    [Fact]
    public void GivenCloseAccount_WhenClosing_ThenThrowException()
    {
        // arrange
        var account = _driver.IsInactive().BuildWithImport().Value;

        // act
        var result = account.Close();

        // assert
        result.Should().BeFailure().And.WithError(AccountErrors.AccountClosed);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GivenAccount_WhenSetAsFavorite_ThenAccountIsModified(bool favorite)
    {
        // arrange
        var account = _driver.IsFavorite(!favorite).Build().Value;

        // act
        var result = account.Modify(favorite: favorite);

        // assert
        result.Should().BeSuccessful();
        result.Value.Favorite.Should().Be(favorite);
    }

    [Fact]
    public void GivenCalculateBalance_WhenCalled_ThenReturnCorrectBalance()
    {
        // arrange
        var gainCategory = CategoryBuilder.GivenACategory().WithId(CategoryId.New).OfType(CategoryType.Gain).Build();
        var expenseCategory = CategoryBuilder.GivenACategory().WithId(CategoryId.New).OfType(CategoryType.Expense).Build();
        var gainTransactions = TransactionBuilder.GivenATransaction().WithId(TransactionId.New).OfAmount(123.45m).ForCategory(gainCategory, true).Build();
        var expenseTransactions = TransactionBuilder.GivenATransaction().WithId(TransactionId.New).OfAmount(12.34m).ForCategory(expenseCategory, true).Build();
        var transactions = new List<Transaction> { gainTransactions, expenseTransactions };

        var account = AccountBuilder.GivenAnActiveAccount()
            .WithId(AccountId.New)
            .OfType(AccountType.Checking)
            .WithOpenBalance(200)
            .WithTransactions(transactions)
            .Build();

        // act
        var balance = account.CalculateBalance();

        // assert
        balance.Should().Be(311.11m);
    }

    private sealed class Driver : IDriverOf<Result<Account>>
    {
        private static readonly Faker Faker = new();
        private AccountId _accountId = AccountId.New;
        private readonly AccountType _accountType = Faker.PickRandom<AccountType>(AccountType.List);
        private string _name = Faker.Lorem.Word();
        private readonly Money _openBalance = MoneyBuilder.Create().Build();
        private DateOnly _openDate = Faker.Date.RecentDateOnly();
        private readonly string _description = Faker.Lorem.Sentence();
        private bool _favorite = Faker.Random.Bool();
        private bool _inactive = Faker.Random.Bool();
        private UserId _userId = (UserId)Faker.Random.Guid();

        public Result<Account> Build() =>
            Account.Create(_accountType,
                _name,
                _openBalance,
                _openDate,
                _description,
                _userId);

        public Result<Account> BuildWithImport() =>
            Account.Import(_accountId,
                _accountType,
                _name,
                _openBalance,
                _openDate,
                _description,
                _favorite,
                _inactive,
                _userId);

        public Driver WithEmptyId()
        {
            _accountId = AccountId.Empty;
            return this;
        }

        public Driver WithName(string name)
        {
            _name = name;
            return this;
        }

        public Driver WithOpenDate(DateOnly openDate)
        {
            _openDate = openDate;
            return this;
        }

        public Driver IsFavorite(bool favorite)
        {
            _favorite = favorite;
            return this;
        }

        public Driver IsInactive()
        {
            _inactive = true;
            return this;
        }

        public Driver WithEmptyUserId()
        {
            _userId = UserId.Empty;
            return this;
        }

        public void ShouldBeValid(Account account)
        {
            using var scope = new AssertionScope();
            account.Id.Should().Be(_accountId);
            account.Type.Should().Be(_accountType);
            account.Name.Should().Be(_name);
            account.OpenBalance.Should().Be(_openBalance);
            account.OpenDate.Should().Be(_openDate);
            account.Favorite.Should().Be(_favorite);
            account.Inactive.Should().Be(_inactive);
            account.UserId.Should().Be(_userId);
        }
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
}
