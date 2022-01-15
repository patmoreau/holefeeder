using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentAssertions;

using MediatR;

using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ReturnsExtensions;

using OneOf;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts
{
    public class CloseAccountCommandHandlerTests
    {
        [Fact]
        public async Task GivenCloseAccountCommand_WhenRequestIsValid_ThenReturnUnit()
        {
            // arrange
            var command = new CloseAccount.Request(Guid.NewGuid());
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(
                    new Account(Guid.NewGuid(), AccountType.Checking, "Account name", DateTime.Today, Guid.NewGuid())
                    {
                        Type = AccountType.Checking,
                        Favorite = false,
                        OpenBalance = Decimal.One,
                        Description = "Description",
                        Inactive = false,
                        Cashflows = Array.Empty<Guid>()
                    });
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var logger = Substitute.For<ILogger<CloseAccount.Handler>>();
            var handler = new CloseAccount.Handler(repository, cache, logger);

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult> expected =
                Unit.Value;
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GivenCloseAccountCommand_WhenAccountDoesNotExists_ThenReturnCommandNotFound()
        {
            // arrange
            var command = new CloseAccount.Request(Guid.NewGuid());
            var repository = Substitute.For<IAccountRepository>();
            repository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var logger = Substitute.For<ILogger<CloseAccount.Handler>>();
            var handler = new CloseAccount.Handler(repository, cache, logger);

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult> expected =
                new NotFoundRequestResult();
            result.Should().BeEquivalentTo(expected);
        }
    }
}
