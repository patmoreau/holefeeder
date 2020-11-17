using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Application.Transactions.Contracts;
using DrifterApps.Holefeeder.Application.Transactions.Models;
using DrifterApps.Holefeeder.Application.Transactions.Queries;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class GetUpcomingHandlerTests
    {
        [Fact]
        public async void GivenHandle_WhenRequestIsNull_ThenThrowArgumentNullException()
        {
            var handler = new GetUpcomingHandler(Substitute.For<IUpcomingQueriesRepository>());

            Func<Task> action = async () => await handler.Handle(null);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async void GivenHandle_WhenRequestIsValid_ThenReturnData()
        {
            var repository = Substitute.For<IUpcomingQueriesRepository>();
            repository.GetUpcomingAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(),
                    CancellationToken.None)
                .Returns(new List<UpcomingViewModel>());

            var handler = new GetUpcomingHandler(repository);


            Func<Task> action = async () =>
                await handler.Handle(new GetUpcomingQuery(Guid.NewGuid(), DateTime.Today, DateTime.Today.AddDays(14)));

            await action.Should().NotThrowAsync();
        }
    }
}
