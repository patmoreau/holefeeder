using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Application.Accounts
{
    public class GetAccountsQueryTests
    {
        [Fact]
        public async Task GivenHandle_WhenRequestIsValid_ThenReturnData()
        {
            // given
            var cache = Substitute.For<ItemsCache>();
            cache["UserId"] = Guid.NewGuid();
            var repository = Substitute.For<IAccountQueriesRepository>();
            repository.FindAsync(Arg.Any<Guid>(), Arg.Any<QueryParams>(), CancellationToken.None)
                .Returns((TestAccountData.Count(), TestAccountData));

            var handler = new GetAccounts.Handler(repository, cache);

            // when
            var result =
                await handler.Handle(
                    new GetAccounts.Request(0, int.MaxValue, Array.Empty<string>(), Array.Empty<string>()), default);

            // then
            var expected = new ListRequestResult(TestAccountData.Count(), TestAccountData);
            result.Value.Should().BeEquivalentTo(expected,
                options => options.ComparingByMembers<OneOf<ValidationErrorsRequestResult, ListRequestResult>>());
        }

        private static IEnumerable<AccountViewModel> TestAccountData
        {
            get
            {
                yield return new AccountViewModel
                {
                    Id = Guid.Parse("58af81e8-78a8-47dc-a2a0-6f4a4c909799"),
                    Type = AccountType.Checking,
                    Name = "name1",
                    TransactionCount = 12345,
                    Balance = Decimal.One,
                    Updated = DateTime.Today,
                    Description = "description1",
                    Favorite = true
                };
                yield return new AccountViewModel
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
            }
        }
    }
}
