using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions.Queries;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Transactions
{
    public class GetTransactionsQueryTests
    {
        [Fact]
        public void GivenQuery_WhenNoQueryParamsPassed_ThenQueryParamsEmptyBuilt()
        {
            // given

            // act
            var query = new GetTransactionsQuery(null, null, null, null);

            // assert
            query.Query.Should().BeEquivalentTo(QueryParams.Empty);
        }

        [Fact]
        public void GivenQuery_WhenQueryValid_ThenValid()
        {
            // given

            // act
            var query = new GetTransactionsQuery(10, 20, new[] {"sort"}, new[] {"filter"});

            // assert
            query.Query.Should().BeEquivalentTo(new QueryParams(10, 20, new[] {"sort"}, new[] {"filter"}));
        }

        [Fact]
        public async Task GivenHandle_WhenRequestIsNull_ThenThrowArgumentNullException()
        {
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var handler = new GetTransactionsHandler(Substitute.For<ITransactionQueriesRepository>(), cache);

            Func<Task> action = async () => await handler.Handle(null, default);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GivenHandle_WhenRequestIsValid_ThenReturnData()
        {
            // given
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var repository = Substitute.For<ITransactionQueriesRepository>();
            repository.FindAsync(Arg.Any<Guid>(), Arg.Any<QueryParams>(), CancellationToken.None)
                .Returns(new QueryResult<TransactionViewModel>(_testTransactionData.Length, _testTransactionData));

            var handler = new GetTransactionsHandler(repository, cache);

            // when
            QueryResult<TransactionViewModel> result = await handler.Handle(new GetTransactionsQuery(null, null, null, null), default);

            // then
            result.Should().BeEquivalentTo(new QueryResult<TransactionViewModel>(2, _testTransactionData));
        }

        private readonly TransactionViewModel[] _testTransactionData =
        {
            new()
            {
                Id = Guid.Parse("6797a22c-6907-45a4-a827-30b5fab7fe0e"),
                Date = DateTime.Today,
                Amount = 12345m,
                Description = "transaction #1",
                Tags = ImmutableArray.Create(new[] {"tag#1", "tag#2"})
            },
            new()
            {
                Id = Guid.Parse("2962edd7-2adf-4fa3-ad97-d3a6bb0447d9"),
                Date = DateTime.Today,
                Amount = 54321m,
                Description = "transaction #2",
                Tags = ImmutableArray.Create(new[] {"tag#1", "tag#2"})
            }
        };
    }
}
