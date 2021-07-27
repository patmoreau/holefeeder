using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.ObjectStore.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using DrifterApps.Holefeeder.ObjectStore.Application.Queries;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace ObjectStore.UnitTests.Application
{
    public class GetItemHandlerTests
    {
        [Fact]
        public async Task GivenHandle_WhenRequestIsNull_ThenThrowArgumentNullException()
        {
            // arrange
            var handler =
                new GetStoreItemHandler(Substitute.For<IStoreItemsQueriesRepository>(), Substitute.For<ItemsCache>());

            // act
            Func<Task> action = async () => await handler.Handle(null, default);

            // assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GivenHandle_WhenRequestIsValid_ThenReturnData()
        {
            // arrange
            const string guid = "934d87f0-a707-49d8-96cc-ecef79b9eac6";

            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();

            var repository = Substitute.For<IStoreItemsQueriesRepository>();
            repository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), CancellationToken.None)
                .Returns(new StoreItemViewModel(Guid.Parse(guid), "code", "data"));

            var handler = new GetStoreItemHandler(repository, cache);

            // act
            var result = await handler.Handle(new GetStoreItemQuery { Id = Guid.NewGuid() }, default);

            // assert
            result.Should().BeEquivalentTo(new StoreItemViewModel(Guid.Parse(guid), "code", "data"));
        }
    }
}
