using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.FunctionalTests.Builders;

using FluentAssertions;
using FluentAssertions.Execution;

using Microsoft.AspNetCore.Mvc;

using Xbehave;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Scenarios
{
    public class AccountScenarios : IClassFixture<BudgetingWebApplicationFactory>
    {
        private readonly BudgetingWebApplicationFactory _factory;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public AccountScenarios(BudgetingWebApplicationFactory factory)
        {
            _factory = factory;

            _factory.SeedData();

            _jsonSerializerOptions = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
        }

        [Scenario]
        public void GivenGetAccounts(HttpClient client, IEnumerable<AccountViewModel>? result)
        {
            "Given GetAccount query"
                .x(() => client = _factory.CreateDefaultClient());

            "When I call the API"
                .x(async () =>
                {
                    const string request = "/api/v2/accounts";

                    result = await client.GetFromJsonAsync<IEnumerable<AccountViewModel>>(request);
                });

            "And the result contain the accounts of the user"
                .x(() => result.Should().NotBeNull().And
                    .HaveCount(3)
                    .And.BeEquivalentTo(new[]
                        {
                            new AccountViewModel
                            {
                                Id = BudgetingContextSeed.Account1,
                                Type = AccountType.Checking,
                                Name = "Account1",
                                OpenBalance = 100.01m,
                                OpenDate = new DateTime(2019, 1, 2),
                                TransactionCount = 6,
                                Balance = 19.01m,
                                Updated = new DateTime(2020, 1, 7),
                                Description = "Description1",
                                Favorite = false
                            },
                            new AccountViewModel
                            {
                                Id = BudgetingContextSeed.Account2,
                                Type = AccountType.CreditCard,
                                Name = "Account2",
                                OpenBalance = 200.02m,
                                OpenDate = new DateTime(2019, 1, 3),
                                TransactionCount = 0,
                                Balance = 200.02m,
                                Updated = new DateTime(2019, 1, 3),
                                Description = "Description2",
                                Favorite = true
                            },
                            new AccountViewModel
                            {
                                Id = BudgetingContextSeed.Account3,
                                Type = AccountType.Loan,
                                Name = "Account3",
                                OpenBalance = 300.03m,
                                OpenDate = new DateTime(2019, 1, 4),
                                TransactionCount = 0,
                                Balance = 300.03m,
                                Updated = new DateTime(2019, 1, 4),
                                Description = "Description3",
                                Favorite = false
                            }
                        }
                    ));
        }

        [Scenario]
        public void GivenGetAccount_WhenWithValidId_ThenReturnResultForUser(HttpClient client, Guid accountId,
            HttpResponseMessage response)
        {
            "Given getting account by id #2"
                .x(() =>
                {
                    client = _factory.CreateClient();
                    accountId = BudgetingContextSeed.Account2;
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
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the account"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<AccountViewModel>(_jsonSerializerOptions);

                    result.Should().BeEquivalentTo(
                        new AccountViewModel
                        {
                            Id = BudgetingContextSeed.Account2,
                            Type = AccountType.CreditCard,
                            Name = "Account2",
                            OpenBalance = 200.02m,
                            OpenDate = new DateTime(2019, 1, 3),
                            TransactionCount = 0,
                            Balance = 200.02m,
                            Updated = new DateTime(2019, 1, 3),
                            Description = "Description2",
                            Favorite = true
                        }
                    );
                });
        }

        [Scenario]
        public void OpenAccountCommand(HttpClient client, OpenAccount.Request command,
            HttpResponseMessage response)
        {
            "Given OpenAccount command"
                .x(() => client = _factory.CreateClient());

            "For newly registered test user"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    Guid.NewGuid().ToString()));

            "With valid data"
                .x(() => command = new OpenAccount.Request(AccountType.Checking, "New Account", DateTime.Today, 1234m,
                    "New account description"));

            "When I call the API"
                .x(async () =>
                {
                    const string requestUri = "/api/v2/accounts/open-account";

                    response = await client.PostAsJsonAsync(requestUri, command);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "With the header location present"
                .x(() =>
                {
                    using (new AssertionScope())
                    {
                        response.Headers.Location.Should().NotBeNull();
                        response.Headers.Location!.AbsolutePath.Should()
                            .MatchRegex(
                                "^/api/v2/accounts/[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$");
                    }
                });

            "And a Guid is returned"
                .x(async () =>
                {
                    var result =
                        await response.Content.ReadFromJsonAsync<JsonElement>(_jsonSerializerOptions);
                    result.GetProperty("id").GetGuid().Should().NotBeEmpty();
                });
        }

        [Scenario]
        public void CloseAccountCommand(HttpClient client, CloseAccount.Request command,
            HttpResponseMessage response)
        {
            "Given CloseAccount command"
                .x(() => client = _factory.CreateClient());

            "For newly registered test user"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    BudgetingContextSeed.TestUserForCommands.ToString()));

            "On Open Account with no cashflows"
                .x(() => command = new CloseAccount.Request(BudgetingContextSeed.OpenAccountNoCashflows));

            "When I call the API"
                .x(async () =>
                {
                    const string requestUri = "/api/v2/accounts/close-account";

                    response = await client.PostAsJsonAsync(requestUri, command);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());
        }

        [Scenario]
        public void CloseAccountCommand_WithActiveCashflows(HttpClient client, CloseAccount.Request command,
            HttpResponseMessage response)
        {
            "Given CloseAccount command"
                .x(() => client = _factory.CreateClient());

            "For newly registered test user"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    BudgetingContextSeed.TestUserForCommands.ToString()));

            "On Open Account with cashflows"
                .x(() => command = new CloseAccount.Request(BudgetingContextSeed.OpenAccountWithCashflows));

            "When I call the API"
                .x(async () =>
                {
                    const string requestUri = "/api/v2/accounts/close-account";

                    response = await client.PostAsJsonAsync(requestUri, command);
                });

            "Then the status code should not indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.StatusCode.Should().Be(HttpStatusCode.BadRequest));

            "And a CommandResult with conflict status"
                .x(async () =>
                {
                    using (new AssertionScope())
                    {
                        var result =
                            await response.Content.ReadFromJsonAsync<ProblemDetails>(_jsonSerializerOptions);
                        result.Should().NotBeNull()
                            .And.BeEquivalentTo(
                                new ProblemDetails
                                {
                                    Detail = "Account entity error: Account has active cashflows", Title = "Account"
                                },
                                options => options.Excluding(info => info.Path.Contains("Type"))
                                    .Excluding(info => info.Path.Contains("Status")));
                    }
                });
        }

        [Scenario]
        public void FavoriteAccountCommand(
            HttpClient client,
            FavoriteAccount.Request command,
            HttpResponseMessage response)
        {
            "Given FavoriteAccount command"
                .x(() =>
                {
                    client = _factory.CreateClient();
                    client.DefaultRequestHeaders.Add(
                        TestAuthHandler.TEST_USER_ID_HEADER,
                        BudgetingContextSeed.TestUserForCommands.ToString());
                    command = new FavoriteAccount.Request(Guid.NewGuid(), true);
                });

            "On Open Account"
                .x(async () =>
                {
                    await _factory.SeedAccountData(_ =>
                        AccountBuilder.Create(command.Id)
                            .OfType(AccountType.Checking)
                            .ForUser(BudgetingContextSeed.TestUserForCommands)
                            .BuildSingle());
                });

            "When I call the API"
                .x(async () =>
                {
                    const string requestUri = "/api/v2/accounts/favorite-account";

                    response = await client.PostAsJsonAsync(requestUri, command);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());
        }

        [Scenario]
        public void ModifyAccountCommand(
            HttpClient client,
            ModifyAccount.Request command,
            HttpResponseMessage response)
        {
            "Given ModifyAccount command"
                .x(() =>
                {
                    client = _factory.CreateClient();
                    client.DefaultRequestHeaders.Add(
                        TestAuthHandler.TEST_USER_ID_HEADER,
                        BudgetingContextSeed.TestUserForCommands.ToString());
                    command = new ModifyAccount.Request(Guid.NewGuid(), "modified name", 123.45m,
                        "modified description");
                });

            "On Open Account"
                .x(async () =>
                {
                    await _factory.SeedAccountData(_ =>
                        AccountBuilder.Create(command.Id)
                            .OfType(AccountType.Checking)
                            .ForUser(BudgetingContextSeed.TestUserForCommands)
                            .BuildSingle());
                });

            "When I call the API"
                .x(async () =>
                {
                    const string requestUri = "/api/v2/accounts/modify-account";

                    response = await client.PostAsJsonAsync(requestUri, command);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());
        }
    }
}
