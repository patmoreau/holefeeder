using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ReturnsExtensions;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts
{
    public class ModifyAccountCommandHandlerTests
    {
        [Fact]
        public async Task GivenModifyAccountCommand_WhenRequestIsValid_ThenReturnCommandOk()
        {
            // arrange
            var command = new ModifyAccountCommand
            {
                Id = Guid.NewGuid(), Name = "New name", OpenBalance = 123.45m, Description = "New description"
            };
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(new Account
                {
                    Id = command.Id,
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
            var logger = Substitute.For<ILogger<ModifyAccountCommandHandler>>();
            var handler = new ModifyAccountCommandHandler(repository, cache, logger);

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            result.Should().BeEquivalentTo(CommandResult.Create(CommandStatus.Ok),
                options => options.ComparingByMembers<CommandResult>());
        }
        
        [Fact]
        public async Task GivenModifyAccountCommand_WhenAccountDoesNotExists_ThenReturnCommandNotFound()
        {
            // arrange
            var command = new ModifyAccountCommand {Id = Guid.NewGuid()};
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var logger = Substitute.For<ILogger<ModifyAccountCommandHandler>>();
            var handler = new ModifyAccountCommandHandler(repository, cache, logger);

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            result.Should().BeEquivalentTo(CommandResult.Create(CommandStatus.NotFound),
                options => options.ComparingByMembers<CommandResult>());
        }
    }
}
