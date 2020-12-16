using System;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;

using DrifterApps.Holefeeder.Budgeting.API;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Infrastructure;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

using MongoDB.Bson;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Scenarios
{
    public class AccountQueriesScenarios : IClassFixture<BudgetingWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public AccountQueriesScenarios(BudgetingWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // [Fact]
        // public async void GivenGetAccounts_WhenNoFilter_ThenReturnResultsForUser()
        // {
        //     // Arrange
        //     var client = _factory.CreateClient();
        //     const string request = "/api/v2/accounts/";
        //
        //     // Act
        //     var response = await client.GetAsync(request);
        //
        //     // Assert
        //     response.StatusCodeShouldBeSuccess();
        //
        //     var result = await response.Content.ReadFromJsonAsync<UpcomingViewModel[]>(_jsonSerializerOptions);
        //
        //     result.Should().BeEquivalentTo(
        //         new AccountViewModel(BudgetingContextSeed.AccountGuid1, AccountType.Checking, "Account1",
        //             5, 130.01m, (new DateTime(2020, 1, 1)).AddDays(5), "Account #1", false),
        //         new AccountViewModel(BudgetingContextSeed.AccountGuid2, AccountType.CreditCard,
        //             "Account2", 0, 200.02m, (new DateTime(2019, 1, 1)).AddDays(2), "Account #2", true),
        //         new AccountViewModel(BudgetingContextSeed.AccountGuid3, AccountType.Loan, "Account3", 0,
        //             300.03m, (new DateTime(2019, 1, 1)).AddDays(3), "Account #3", false)
        //     );
        // }
    }
}
