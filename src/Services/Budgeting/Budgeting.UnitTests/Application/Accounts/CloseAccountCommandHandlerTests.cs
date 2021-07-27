using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ReturnsExtensions;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts
{
    public class CloseAccountCommandHandlerTests
    {
        [Fact]
        public async Task GivenCloseAccountCommand_WhenRequestIsValid_ThenReturnCommandOk()
        {
            // arrange
            var command = new CloseAccountCommand {Id = Guid.NewGuid()};
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(new Account
                {
                    Id = Guid.NewGuid(),
                    Type = AccountType.Checking,
                    Name = "Account name",
                    Favorite = false,
                    OpenBalance = Decimal.One,
                    OpenDate = DateTime.Today,
                    Description = "Description",
                    Inactive = false,
                    UserId = Guid.NewGuid(),
                    Cashflows = Array.Empty<Guid>()
                });
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var logger = Substitute.For<ILogger<CloseAccountCommandHandler>>();
            var handler = new CloseAccountCommandHandler(repository, cache, logger);

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            result.Should().BeEquivalentTo(CommandResult.Create(CommandStatus.Ok),
                options => options.ComparingByMembers<CommandResult>());
        }
        
        [Fact]
        public async Task GivenCloseAccountCommand_WhenAccountDoesNotExists_ThenReturnCommandNotFound()
        {
            // arrange
            var command = new CloseAccountCommand {Id = Guid.NewGuid()};
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var logger = Substitute.For<ILogger<CloseAccountCommandHandler>>();
            var handler = new CloseAccountCommandHandler(repository, cache, logger);

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            result.Should().BeEquivalentTo(CommandResult.Create(CommandStatus.NotFound),
                options => options.ComparingByMembers<CommandResult>());
        }
    }
}
