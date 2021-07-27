using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;

using FluentAssertions;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Domain
{
    public class AccountContextTests
    {
        [Fact]
        public void GivenCloseAccount_WhenClosing_ThenThrowException()
        {
            // arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Type = AccountType.Checking,
                Name = "Account name",
                Favorite = false,
                OpenBalance = Decimal.One,
                OpenDate = DateTime.Today,
                Description = "Description",
                Inactive = true,
                UserId = Guid.NewGuid(),
                Cashflows = Array.Empty<Guid>()
            };

            // act
            Action action = () => account.Close();

            // assert
            action.Should().Throw<HolefeederDomainException>().WithMessage("Account already closed");
        }

        [Fact]
        public void GivenOpenAccountWithCashflows_WhenClosing_ThenThrowException()
        {
            // arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Type = AccountType.Checking,
                Name = "Account name",
                Favorite = false,
                OpenBalance = Decimal.One,
                OpenDate = DateTime.Today,
                Description = "Description",
                Inactive = false,
                UserId = Guid.NewGuid(),
                Cashflows = new[] {Guid.NewGuid()}
            };

            // act
            Action action = () => account.Close();

            // assert
            action.Should().Throw<HolefeederDomainException>().WithMessage("Account has active cashflows");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GivenAccount_WhenSetAsFavorite_ThenAccountIsModified(bool favorite)
        {
            // arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Type = AccountType.Checking,
                Name = "Account name",
                Favorite = false,
                OpenBalance = Decimal.One,
                OpenDate = DateTime.Today,
                Description = "Description",
                Inactive = false,
                UserId = Guid.NewGuid(),
                Cashflows = new[] {Guid.NewGuid()}
            };

            // act
            account = account with {Favorite = favorite};

            // assert
            account.Favorite.Should().Be(favorite);
        }
    }
}
