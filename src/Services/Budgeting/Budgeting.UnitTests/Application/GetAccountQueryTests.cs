using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Queries;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application
{
    public class GetAccountQueryTests
    {
        [Fact]
        public void GivenQuery_WhenUserIdEmpty_ThenThrowArgumentNullException()
        {
            // given
            
            // act
            Action action = () => new GetAccountQuery(Guid.Empty, Guid.NewGuid());
            
            // assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenQuery_WhenIdEmpty_ThenThrowArgumentNullException()
        {
            // given
            
            // act
            Action action = () => new GetAccountQuery(Guid.NewGuid(), Guid.Empty);
            
            // assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenQuery_WhenQueryValid_ThenValid()
        {
            // given
            
            // act
            var query = new GetAccountQuery(Guid.NewGuid(), Guid.NewGuid());
            
            // assert
            query.UserId.Should().NotBeEmpty();
            query.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async void GivenHandle_WhenRequestIsNull_ThenThrowArgumentNullException()
        {
            // given
            var handler = new GetAccountHandler(Substitute.For<IAccountQueriesRepository>());

            // act
            Func<Task> action = async () => await handler.Handle(null);

            // assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async void GivenHandle_WhenRequestIsValid_ThenReturnData()
        {
            // given
            var repository = Substitute.For<IAccountQueriesRepository>();
            repository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), CancellationToken.None)
                .Returns(TestAccountData);

            var handler = new GetAccountHandler(repository);

            // when
            var result = await handler.Handle(new GetAccountQuery(Guid.NewGuid(), Guid.NewGuid()));

            // then
            result.Should().BeEquivalentTo(TestAccountData);
        }

        private static AccountViewModel TestAccountData
        {
            get
            {
                return new AccountViewModel(Guid.Parse("4a35e373-45b1-43f7-98cf-09960d96f191"),
                    AccountType.Checking, "name2", 54321, Decimal.One, DateTime.Today, "description2", true);
            }
        }
    }
}
