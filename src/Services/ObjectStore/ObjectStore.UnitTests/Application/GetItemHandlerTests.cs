using System;
using System.Threading;
using System.Threading.Tasks;
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
        public async void GivenHandle_WhenRequestIsNull_ThenThrowArgumentNullException()
        {
            var handler = new GetStoreItemHandler(Substitute.For<IStoreQueriesRepository>());

            Func<Task> action = async () => await handler.Handle(null);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async void GivenHandle_WhenRequestIsValid_ThenReturnData()
        {
            const string guid = "934d87f0-a707-49d8-96cc-ecef79b9eac6";
            
            var repository = Substitute.For<IStoreQueriesRepository>();
            repository.GetItemAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), CancellationToken.None)
                .Returns(new StoreItemViewModel(Guid.Parse(guid), "code", "data"));
            
            var handler = new GetStoreItemHandler(repository);

            var result = await handler.Handle(new GetStoreItemQuery(Guid.NewGuid(), Guid.NewGuid()));

            result.Should().BeEquivalentTo(new StoreItemViewModel(Guid.Parse(guid), "code", "data"));
        }
    }
}
