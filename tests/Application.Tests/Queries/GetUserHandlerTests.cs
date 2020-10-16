using System;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Models;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class GetUserHandlerTests
    {
        [Fact]
        public void GivenConstructor_WhenRepositoryIsNull_ThenThrowArgumentNullException()
        {
            Action action = () => _ = new GetUserHandler(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async void GivenHandle_WhenRequestIsNull_ThenThrowArgumentNullException()
        {
            var handler = new GetUserHandler(Substitute.For<IUserQueriesRepository>());

            Func<Task> action = async () => await handler.Handle(null);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async void GivenHandle_WhenRequestIsValid_ThenReturnData()
        {
            var repository = Substitute.For<IUserQueriesRepository>();
            repository.GetUserByEmailAsync(Arg.Any<string>()).Returns((UserViewModel)null);

            var handler = new GetUserHandler(repository);


            Func<Task> action = async () =>
                await handler.Handle(new GetUserQuery("user@email.com"));

            await action.Should().NotThrowAsync();
        }
    }
}
