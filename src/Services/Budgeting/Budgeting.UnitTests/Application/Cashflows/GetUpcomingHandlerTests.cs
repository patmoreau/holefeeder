using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Cashflows;
using DrifterApps.Holefeeder.Budgeting.Application.Cashflows.Queries;
using DrifterApps.Holefeeder.Budgeting.Application.Models;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Cashflows
{
    public class GetUpcomingHandlerTests
    {
        [Fact]
        public async Task GivenHandle_WhenRequestIsValid_ThenReturnData()
        {
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var repository = Substitute.For<IUpcomingQueriesRepository>();
            repository.GetUpcomingAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(),
                    CancellationToken.None)
                .Returns(new List<UpcomingViewModel>());

            var handler = new GetUpcoming.Handler(repository, cache);


            Func<Task> action = async () =>
                await handler.Handle(new GetUpcoming.Request(DateTime.Today, DateTime.Today.AddDays(14)), default);

            await action.Should().NotThrowAsync();
        }
    }
}
