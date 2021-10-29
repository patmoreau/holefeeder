using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.FunctionalTests.Builders;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using FluentAssertions;
using FluentAssertions.Execution;

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

            _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [Scenario]
        public void GivenGetAccounts(HttpClient client, HttpResponseMessage response)
        {
            "Given GetAccount query"
                .x(() => client = _factory.CreateDefaultClient());

            "When I call the API"
                .x(async () =>
                {
                    const string request = "/api/v2/accounts/get-accounts";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the accounts of the user"
                .x(async () =>
                {
                    var result =
                        await response.Content.ReadFromJsonAsync<QueryResult<AccountViewModel>>(_jsonSerializerOptions);

                    using (new AssertionScope())
                    {
                        result.Should().NotBeNull();
                        result?.TotalCount.Should().Be(3);
                        result?.Items.Should()
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
                            );
                    }
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
        public void OpenAccountCommand(HttpClient client, OpenAccountCommand command,
            HttpResponseMessage response)
        {
            "Given OpenAccount command"
                .x(() => client = _factory.CreateClient());

            "For newly registered test user"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    Guid.NewGuid().ToString()));

            "With valid data"
                .x(() => command = new OpenAccountCommand
                {
                    Type = AccountType.Checking,
                    Name = "New Account",
                    OpenBalance = 1234m,
                    OpenDate = DateTime.Today,
                    Description = "New account description"
                });

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
                .x(() => response.Headers.Location?.AbsolutePath.Should().StartWithEquivalentOf("/api/v2/accounts/"));

            "And a CommandResult with created status"
                .x(async () =>
                {
                    using (new AssertionScope())
                    {
                        var result =
                            await response.Content.ReadFromJsonAsync<CommandResult<Guid>>(_jsonSerializerOptions);
                        result.Should().NotBeNull()
                            .And.BeEquivalentTo(CommandResult<Guid>.Create(CommandStatus.Created, Guid.Empty),
                                options => options
                                    .ComparingByMembers<CommandResult<Guid>>()
                                    .Using<Guid>(ctx => ctx.Subject.Should().NotBeEmpty()).WhenTypeIs<Guid>());
                    }
                });
        }

        [Scenario]
        public void CloseAccountCommand(HttpClient client, CloseAccountCommand command,
            HttpResponseMessage response)
        {
            "Given CloseAccount command"
                .x(() => client = _factory.CreateClient());

            "For newly registered test user"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    BudgetingContextSeed.TestUserForCommands.ToString()));

            "On Open Account with no cashflows"
                .x(() => command = new CloseAccountCommand { Id = BudgetingContextSeed.OpenAccountNoCashflows });

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

            "And a CommandResult with ok status"
                .x(async () =>
                {
                    using (new AssertionScope())
                    {
                        var result =
                            await response.Content.ReadFromJsonAsync<CommandResult>(_jsonSerializerOptions);
                        result.Should().NotBeNull()
                            .And.BeEquivalentTo(CommandResult.Create(CommandStatus.Ok),
                                options => options.ComparingByMembers<CommandResult>());
                    }
                });
        }

        [Scenario]
        public void CloseAccountCommand_WithActiveCashflows(HttpClient client, CloseAccountCommand command,
            HttpResponseMessage response)
        {
            "Given CloseAccount command"
                .x(() => client = _factory.CreateClient());

            "For newly registered test user"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    BudgetingContextSeed.TestUserForCommands.ToString()));

            "On Open Account with cashflows"
                .x(() => command = new CloseAccountCommand { Id = BudgetingContextSeed.OpenAccountWithCashflows });

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
                            await response.Content.ReadFromJsonAsync<CommandResult>(_jsonSerializerOptions);
                        result.Should().NotBeNull()
                            .And.BeEquivalentTo(
                                CommandResult.Create(CommandStatus.Conflict, "Account has active cashflows"),
                                options => options.ComparingByMembers<CommandResult>());
                    }
                });
        }

        [Scenario]
        public void FavoriteAccountCommand(
            HttpClient client,
            FavoriteAccountCommand command,
            HttpResponseMessage response)
        {
            "Given FavoriteAccount command"
                .x(() =>
                {
                    client = _factory.CreateClient();
                    client.DefaultRequestHeaders.Add(
                        TestAuthHandler.TEST_USER_ID_HEADER,
                        BudgetingContextSeed.TestUserForCommands.ToString());
                    command = new FavoriteAccountCommand { Id = Guid.NewGuid(), IsFavorite = true };
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
            ModifyAccountCommand command,
            HttpResponseMessage response)
        {
            "Given ModifyAccount command"
                .x(() =>
                {
                    client = _factory.CreateClient();
                    client.DefaultRequestHeaders.Add(
                        TestAuthHandler.TEST_USER_ID_HEADER,
                        BudgetingContextSeed.TestUserForCommands.ToString());
                    command = new ModifyAccountCommand
                    {
                        Id = Guid.NewGuid(),
                        Name = "modified name",
                        Description = "modified description",
                        OpenBalance = 123.45m
                    };
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
