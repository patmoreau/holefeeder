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

using OneOf;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Transactions;

public class GetTransactionsQueryTests
{
    private readonly TransactionInfoViewModel[] _testTransactionData =
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

    [Fact]
    public async Task GivenHandle_WhenRequestIsValid_ThenReturnData()
    {
        // given
        var cache = Substitute.For<ItemsCache>();
        cache["UserId"] = Guid.NewGuid();
        var repository = Substitute.For<ITransactionQueriesRepository>();
        repository.FindAsync(Arg.Any<Guid>(), Arg.Any<QueryParams>(), CancellationToken.None)
            .Returns((_testTransactionData.Length, _testTransactionData));

        var handler = new GetTransactions.Handler(repository, cache);

        // when
        var result =
            await handler.Handle(new GetTransactions.Request(0, 1, Array.Empty<string>(), Array.Empty<string>()),
                default);

        // then
        OneOf<ValidationErrorsRequestResult, ListRequestResult> expected =
            new ListRequestResult(2, _testTransactionData);
        result.Should().BeEquivalentTo(expected);
    }
}
