using System;
using System.Linq;

using Bogus;
using Bogus.Extensions;

using FluentAssertions;

using Holefeeder.Domain.Features.Accounts;

using Xunit;

namespace Holefeeder.UnitTests.Domain.Features.Accounts;

public class AccountTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void GivenConstructor_WhenIdEmpty_ThenThrowException()
    {
        // arrange

        // act
        Action action = () => _ = new Account(Guid.Empty,
            _faker.PickRandom(AccountType.List.ToArray()),
            _faker.Random.Word(),
            _faker.Date.Recent(),
            _faker.Random.Guid());

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("Id is required")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenConstructor_WhenNameIsEmpty_ThenThrowException(string name)
    {
        // arrange

        // act
        Action action = () => _ = new Account(_faker.Random.Guid(),
            _faker.PickRandom(AccountType.List.ToArray()),
            name,
            _faker.Date.Recent(),
            _faker.Random.Guid());

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("Name must be from 1 to 100 characters")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Fact]
    public void GivenConstructor_WhenNameIsTooLong_ThenThrowException()
    {
        // arrange

        // act
        Action action = () => _ = new Account(_faker.Random.Guid(),
            _faker.PickRandom(AccountType.List.ToArray()),
            _faker.Random.Words().ClampLength(256),
            _faker.Date.Recent(),
            _faker.Random.Guid());

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("Name must be from 1 to 100 characters")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Fact]
    public void GivenConstructor_WhenOpenDateIsMissing_ThenThrowException()
    {
        // arrange

        // act
        Action action = () => _ = new Account(_faker.Random.Guid(),
            _faker.PickRandom(AccountType.List.ToArray()),
            _faker.Random.Word(),
            default,
            _faker.Random.Guid());

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("OpenDate is required")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Fact]
    public void GivenConstructor_WhenUserIdEmpty_ThenThrowException()
    {
        // arrange

        // act
        Action action = () => _ = new Account(_faker.Random.Guid(),
            _faker.PickRandom(AccountType.List.ToArray()),
            _faker.Random.Word(),
            _faker.Date.Recent(),
            Guid.Empty);

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("UserId is required")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Fact]
    public void GivenCloseAccount_WhenClosing_ThenThrowException()
    {
        // arrange
        var account =
            new Account(Guid.NewGuid(), AccountType.Checking, "Account name", DateTime.Today, Guid.NewGuid())
            {
                Favorite = false,
                OpenBalance = Decimal.One,
                Description = "Description",
                Inactive = true,
                Cashflows = Array.Empty<Guid>()
            };

        // act
        Action action = () => account.Close();

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("Account already closed")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Fact]
    public void GivenOpenAccountWithCashflows_WhenClosing_ThenThrowException()
    {
        // arrange
        var account =
            new Account(Guid.NewGuid(), AccountType.Checking, "Account name", DateTime.Today, Guid.NewGuid())
            {
                Favorite = false,
                OpenBalance = Decimal.One,
                Description = "Description",
                Inactive = false,
                Cashflows = new[] {Guid.NewGuid()}
            };

        // act
        Action action = () => account.Close();

        // assert
        action.Should().Throw<AccountDomainException>()
            .WithMessage("Account has active cashflows")
            .And
            .Context.Should().Be(nameof(Account));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GivenAccount_WhenSetAsFavorite_ThenAccountIsModified(bool favorite)
    {
        // arrange
        var account =
            new Account(Guid.NewGuid(), AccountType.Checking, "Account name", DateTime.Today, Guid.NewGuid())
            {
                Favorite = false,
                OpenBalance = Decimal.One,
                Description = "Description",
                Inactive = false,
                Cashflows = new[] {Guid.NewGuid()}
            };

        // act
        account = account with {Favorite = favorite};

        // assert
        account.Favorite.Should().Be(favorite);
    }
}
