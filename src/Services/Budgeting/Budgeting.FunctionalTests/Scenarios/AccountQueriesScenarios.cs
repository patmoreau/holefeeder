using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using DrifterApps.Holefeeder.Budgeting.API;
using DrifterApps.Holefeeder.Budgeting.Application.Converters;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Infrastructure;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

using MongoDB.Bson;

using Xbehave;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Scenarios
{
    public class AccountQueriesScenarios : IClassFixture<BudgetingWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new() {PropertyNameCaseInsensitive = true,};

        public AccountQueriesScenarios(BudgetingWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Scenario]
        public void GivenGetAccounts_WhenNoFilter_ThenReturnResultsForUser(HttpClient client,
            HttpResponseMessage response)
        {
            "Given getting accounts"
                .x(() => client = _factory.CreateClient());

            "For user TestUser #1"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    BudgetingContextSeed.TestUserGuid1.ToString()));

            "When I get call the API"
                .x(async () =>
                {
                    string request = $"/api/v2/accounts/get-accounts";

                    // Act
                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.StatusCodeShouldBeSuccess());

            "And the result contain the accounts of the user"
                .x(async () =>
                {
                    var jsonOptions = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
                    jsonOptions.Converters.Add(new AccountTypeConverter());
                    jsonOptions.Converters.Add(new CategoryTypeConverter());
                    var result = await response.Content.ReadFromJsonAsync<AccountViewModel[]>(jsonOptions);

                    result.Should().BeEquivalentTo(
                        new AccountViewModel(BudgetingContextSeed.AccountGuid1, AccountType.Checking, "Account1", 7,
                            100.01m, new DateTime(2020, 1, 8), "Description1", false),
                        new AccountViewModel(BudgetingContextSeed.AccountGuid2, AccountType.CreditCard, "Account2", 0,
                            200.02m, new DateTime(2019, 1, 3), "Description2", true),
                        new AccountViewModel(BudgetingContextSeed.AccountGuid3, AccountType.Loan, "Account3", 0,
                            300.03m, new DateTime(2019, 1, 4), "Description3", false)
                    );
                });
        }

        [Scenario]
        public void GivenGetAccount_WhenWithValidId_ThenReturnResultForUser(HttpClient client, Guid accountId,
            HttpResponseMessage response)
        {
            "Given getting account by id #2"
                .x(() =>
                {
                    client = _factory.CreateClient();
                    accountId = BudgetingContextSeed.AccountGuid2;
                });

            "For user TestUser #1"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    BudgetingContextSeed.TestUserGuid1.ToString()));

            "When I get call the API"
                .x(async () =>
                {
                    string request = $"/api/v2/accounts/{accountId.ToString()}";

                    // Act
                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.StatusCodeShouldBeSuccess());

            "And the result contain the accounts of the user"
                .x(async () =>
                {
                    var jsonOptions = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
                    jsonOptions.Converters.Add(new AccountTypeConverter());
                    jsonOptions.Converters.Add(new CategoryTypeConverter());
                    var result = await response.Content.ReadFromJsonAsync<AccountViewModel>(jsonOptions);

                    result.Should().BeEquivalentTo(
                        new AccountViewModel(BudgetingContextSeed.AccountGuid2, AccountType.CreditCard, "Account2", 0,
                            200.02m, new DateTime(2019, 1, 3), "Description2", true)
                    );
                });
        }
    }
}
