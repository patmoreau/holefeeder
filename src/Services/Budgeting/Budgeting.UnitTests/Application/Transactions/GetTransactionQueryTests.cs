using System;
using System.Collections.Generic;
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

using OneOf;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Transactions
{
    public class GetTransactionQueryTests
    {
        [Theory]
        [MemberData(nameof(GetIds))]
        public void GivenQuery_WhenQueryValid_ThenValid(Guid id)
        {
            // given

            // act
            var query = new GetTransaction.Request(id);

            // assert
            query.Id.Should().Be(id);
        }

        [Fact]
        public async Task GivenHandle_WhenRequestIsValid_ThenReturnData()
        {
            // given
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var repository = Substitute.For<ITransactionQueriesRepository>();
            repository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), CancellationToken.None)
                .Returns(_testTransactionData[0] with { });

            var handler = new GetTransaction.Handler(repository, cache);

            // when
            var result = await handler.Handle(new GetTransaction.Request(Guid.NewGuid()), default);

            // then
            OneOf<TransactionViewModel, NotFoundRequestResult> expected = _testTransactionData[0];
            result.Should().BeEquivalentTo(expected);
        }

        public static IEnumerable<object[]> GetIds()
        {
            yield return new object[] { Guid.NewGuid() };
            yield return new object[] { Guid.Empty };
        }

        private readonly TransactionViewModel[] _testTransactionData =
        {
            new()
            {
                Id = Guid.Parse("6797a22c-6907-45a4-a827-30b5fab7fe0e"),
                Date = DateTime.Today,
                Amount = 12345m,
                Description = "transaction #1",
                Tags = ImmutableArray.Create(new[] { "tag#1", "tag#2" })
            },
            new()
            {
                Id = Guid.Parse("2962edd7-2adf-4fa3-ad97-d3a6bb0447d9"),
                Date = DateTime.Today,
                Amount = 54321m,
                Description = "transaction #2",
                Tags = ImmutableArray.Create(new[] { "tag#1", "tag#2" })
            }
        };
    }
}
