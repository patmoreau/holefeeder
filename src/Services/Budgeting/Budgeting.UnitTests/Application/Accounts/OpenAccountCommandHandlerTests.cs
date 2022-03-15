using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

using FluentAssertions;

using FluentValidation;

using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ReturnsExtensions;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts;

public class OpenAccountCommandHandlerTests
{
    [Fact]
    public async Task GivenOpenAccountCommand_WhenRequestIsValid_ThenReturnCommandCreatedWithId()
    {
        // arrange
        var command = new OpenAccount.Request(AccountType.Checking, "new account", DateTime.Today, Decimal.One,
            "description");
        var repository = Substitute.For<IAccountRepository>();
        repository.FindByNameAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();
        var cache = Substitute.For<ItemsCache>();
        cache["UserId"] = Guid.NewGuid();
        var logger = Substitute.For<ILogger<OpenAccount.Handler>>();
        var handler = new OpenAccount.Handler(repository, cache, logger);

        // act
        var result = await handler.Handle(command, CancellationToken.None);

        // assert
        result.Value.Should().BeAssignableTo<Guid>().And.NotBe(Guid.Empty);
    }

    [Fact]
    public void GivenOpenAccountCommand_WhenAccountNameAlreadyExists_ThenReturnCommandConflict()
    {
        // arrange
        var command = new OpenAccount.Request(AccountType.Checking, "new account", DateTime.Today, Decimal.One,
            "description");
        var repository = Substitute.For<IAccountRepository>();
        repository.FindByNameAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(
                new Account(Guid.NewGuid(), AccountType.Checking, "new account", DateTime.Today, Guid.NewGuid())
                {
                    Favorite = false,
                    OpenBalance = Decimal.One,
                    Description = "description",
                    Inactive = false,
                    Cashflows = Array.Empty<Guid>()
                });
        var cache = Substitute.For<ItemsCache>();
        cache["UserId"] = Guid.NewGuid();
        var logger = Substitute.For<ILogger<OpenAccount.Handler>>();
        var handler = new OpenAccount.Handler(repository, cache, logger);

        // act
        Func<Task> action = async () => await handler.Handle(command, CancellationToken.None);

        // assert
        action.Should().ThrowExactlyAsync<ValidationException>()
            .WithMessage("Account name 'new account' already exists");
    }
}
