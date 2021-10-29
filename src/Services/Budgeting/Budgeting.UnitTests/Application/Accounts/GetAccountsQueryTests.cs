using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Queries;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts
{
    public class GetAccountsQueryTests
    {
        [Fact]
        public void GivenQuery_WhenNoQueryParamsPassed_ThenQueryParamsEmptyBuilt()
        {
            // given

            // act
            var query = new GetAccountsQuery(null, null, null, null);

            // assert
            query.Query.Should().BeEquivalentTo(QueryParams.Empty);
        }

        [Fact]
        public void GivenQuery_WhenQueryValid_ThenValid()
        {
            // given

            // act
            var query = new GetAccountsQuery(10, 20, new[] { "sort" }, new[] { "filter" });

            // assert
            query.Query.Should().BeEquivalentTo(new QueryParams(10, 20, new[] { "sort" }, new[] { "filter" }));
        }

        [Fact]
        public async Task GivenHandle_WhenRequestIsNull_ThenThrowArgumentNullException()
        {
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var handler = new GetAccountsHandler(Substitute.For<IAccountQueriesRepository>(), cache);

            Func<Task> action = async () => await handler.Handle(null, default);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GivenHandle_WhenRequestIsValid_ThenReturnData()
        {
            // given
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var repository = Substitute.For<IAccountQueriesRepository>();
            repository.FindAsync(Arg.Any<Guid>(), Arg.Any<QueryParams>(), CancellationToken.None)
                .Returns(new QueryResult<AccountViewModel>(TestAccountData.Count(), TestAccountData));

            var handler = new GetAccountsHandler(repository, cache);

            // when
            var result = await handler.Handle(new GetAccountsQuery(null, null, null, null), default);

            // then
            result.Should().BeEquivalentTo(new QueryResult<AccountViewModel>(TestAccountData.Count(), TestAccountData));
        }

        private static IEnumerable<AccountViewModel> TestAccountData
        {
            get
            {
                yield return new AccountViewModel
                {
                    Id = Guid.Parse("58af81e8-78a8-47dc-a2a0-6f4a4c909799"),
                    Type = AccountType.Checking,
                    Name = "name1",
                    TransactionCount = 12345,
                    Balance = Decimal.One,
                    Updated = DateTime.Today,
                    Description = "description1",
                    Favorite = true
                };
                yield return new AccountViewModel
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
