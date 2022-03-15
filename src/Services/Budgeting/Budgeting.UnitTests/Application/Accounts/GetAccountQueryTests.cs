using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Queries;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentAssertions;

using NSubstitute;

using OneOf;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts;

public class GetAccountQueryTests
{
    private static AccountViewModel TestAccountData =>
        new AccountViewModel
        {
            Id = Guid.Parse("4a35e373-45b1-43f7-98cf-09960d96f191"),
            Type = AccountType.Checking,
            Name = "name2",
            TransactionCount = 54321,
            Balance = Decimal.One,
            Updated = DateTime.Today,
            Description = "description2",
            Favorite = true
        };

    [Fact]
    public async Task GivenHandle_WhenRequestIsValid_ThenReturnData()
    {
        // given
        var cache = Substitute.For<ItemsCache>();
        cache["UserId"] = Guid.NewGuid();
        var repository = Substitute.For<IAccountQueriesRepository>();
        repository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), CancellationToken.None)
            .Returns(TestAccountData);

        var handler = new GetAccount.Handler(repository, cache);

        // when
        var result = await handler.Handle(new GetAccount.Request(Guid.NewGuid()), default);

        // then
        OneOf<AccountViewModel, NotFoundRequestResult> expected = TestAccountData;
        result.Should().BeEquivalentTo(expected);
    }
}
