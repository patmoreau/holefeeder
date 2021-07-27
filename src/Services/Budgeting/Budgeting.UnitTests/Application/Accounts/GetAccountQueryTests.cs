using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Queries;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts
{
    public class GetAccountQueryTests
    {
        [Fact]
        public void GivenQuery_WhenIdEmpty_ThenThrowArgumentNullException()
        {
            // given

            // act
            Action action = () => new GetAccountQuery(Guid.Empty);

            // assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenQuery_WhenQueryValid_ThenValid()
        {
            // given

            // act
            var query = new GetAccountQuery(Guid.NewGuid());

            // assert
            query.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GivenHandle_WhenRequestIsNull_ThenThrowArgumentNullException()
        {
            // given
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var handler = new GetAccountHandler(Substitute.For<IAccountQueriesRepository>(), cache);

            // act
            Func<Task> action = async () => await handler.Handle(null, default);

            // assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GivenHandle_WhenRequestIsValid_ThenReturnData()
        {
            // given
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var repository = Substitute.For<IAccountQueriesRepository>();
            repository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), CancellationToken.None)
                .Returns(TestAccountData);

            var handler = new GetAccountHandler(repository, cache);

            // when
            var result = await handler.Handle(new GetAccountQuery(Guid.NewGuid()), default);

            // then
            result.Should().BeEquivalentTo(TestAccountData);
        }

        private static AccountViewModel TestAccountData
        {
            get
            {
                return new AccountViewModel
                {
                    Id = Guid.Parse("4a35e373-45b1-43f7-98cf-09960d96f191"),
                    Type = AccountType.Checking,
                    Name = "name2",
                    TransactionCount = 54321,
                    Balance = Decimal.One,
                    Updated = DateTime.Today,
                    Description = "description2",
                    Favorite = true
                };
            }
        }
    }
}
