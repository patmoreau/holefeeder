using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems;
using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Queries;

using FluentAssertions;

using NSubstitute;

using OneOf;

using Xunit;

namespace ObjectStore.UnitTests.Application
{
    public class GetItemHandlerTests
    {
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

            var handler = new GetStoreItem.Handler(repository, cache);

            // act
            var result = await handler.Handle(new GetStoreItem.Request {Id = Guid.NewGuid()}, default);

            // assert
            OneOf<StoreItemViewModel, NotFoundRequestResult> expected =
                new StoreItemViewModel(Guid.Parse(guid), "code", "data");
            result.Should()
                .BeEquivalentTo(expected);
        }
    }
}
