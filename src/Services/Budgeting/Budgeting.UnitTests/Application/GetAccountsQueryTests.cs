using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Queries;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application
{
    public class GetAccountsQueryTests
    {
        [Fact]
        public void GivenQuery_WhenUserIdEmpty_ThenThrowArgumentNullException()
        {
            // given

            // act
            Action action = () => new GetAccountsQuery(Guid.Empty, QueryParams.DefaultOffset, QueryParams.DefaultLimit,
                QueryParams.DefaultSort, QueryParams.DefaultFilter);

            // assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenQuery_WhenQueryParamsPassed_ThenQueryParamsBuilt()
        {
            // given

            // act
            Action action = () => new GetAccountsQuery(Guid.Empty, QueryParams.DefaultOffset, QueryParams.DefaultLimit,
                QueryParams.DefaultSort, QueryParams.DefaultFilter);

            // assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenQuery_WhenNoQueryParamsPassed_ThenQueryParamsEmptyBuilt()
        {
            // given

            // act
            var query = new GetAccountsQuery(Guid.NewGuid(), null, null, null, null);

            // assert
            query.Query.Should().BeEquivalentTo(QueryParams.Empty);
        }

        [Fact]
        public void GivenQuery_WhenQueryValid_ThenValid()
        {
            // given

            // act
            var query = new GetAccountsQuery(Guid.NewGuid(), 10, 20, new[] {"sort"}, new[] {"filter"});

            // assert
            query.UserId.Should().NotBeEmpty();
            query.Query.Should().BeEquivalentTo(new QueryParams(10, 20, new[] {"sort"}, new[] {"filter"}));
        }

        [Fact]
        public async void GivenHandle_WhenRequestIsNull_ThenThrowArgumentNullException()
        {
            var handler = new GetAccountsHandler(Substitute.For<IAccountQueriesRepository>());

            Func<Task> action = async () => await handler.Handle(null);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async void GivenHandle_WhenRequestIsValid_ThenReturnData()
        {
            // given
            var repository = Substitute.For<IAccountQueriesRepository>();
            repository.FindAsync(Arg.Any<Guid>(), Arg.Any<QueryParams>(), CancellationToken.None)
                .Returns(TestAccountData);

            var handler = new GetAccountsHandler(repository);

            // when
            var result = await handler.Handle(new GetAccountsQuery(Guid.NewGuid(), null, null, null, null));

            // then
            result.Should().BeEquivalentTo(TestAccountData);
        }

        private static IEnumerable<AccountViewModel> TestAccountData
        {
            get
            {
                yield return new AccountViewModel(Guid.Parse("58af81e8-78a8-47dc-a2a0-6f4a4c909799"),
                    AccountType.Checking, "name1", 12345, Decimal.One, DateTime.Today, "description1", true);
                yield return new AccountViewModel(Guid.Parse("4a35e373-45b1-43f7-98cf-09960d96f191"),
                    AccountType.Checking, "name2", 54321, Decimal.One, DateTime.Today, "description2", true);
            }
        }
    }
}
