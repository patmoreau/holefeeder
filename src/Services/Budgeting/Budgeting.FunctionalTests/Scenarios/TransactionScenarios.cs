using System;
using System.Collections.Immutable;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using FluentAssertions;
using FluentAssertions.Execution;

using Xbehave;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Scenarios
{
    public class TransactionScenarios : IClassFixture<BudgetingWebApplicationFactory>
    {
        private readonly BudgetingWebApplicationFactory _factory;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public TransactionScenarios(BudgetingWebApplicationFactory factory)
        {
            _factory = factory;
            
            _factory.SeedData();

            _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [Scenario]
        public void GivenGetTransactions(HttpClient client, HttpResponseMessage response)
        {
            "Given GetTransactions query"
                .x(() => client = _factory.CreateDefaultClient());

            "When I call the API"
                .x(async () =>
                {
                    const string request = "/api/v2/transactions/get-transactions";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the transactions of the user"
                .x(async () =>
                {
                    var result =
                        await response.Content.ReadFromJsonAsync<QueryResult<TransactionViewModel>>(
                            _jsonSerializerOptions);

                    using (new AssertionScope())
                    {
                        result.Should().NotBeNull();
                        result?.TotalCount.Should().Be(6);
                        result?.Items.Should()
                            .HaveCount(6)
                            .And.BeEquivalentTo(new[]
                                {
                                    new TransactionViewModel
                                    {
                                        Id = BudgetingContextSeed.Transaction1,
                                        Date = new DateTime(2020, 1, 2),
                                        Amount = 10m,
                                        Description = "Transaction1",
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1,
                                            "Category1", CategoryType.Expense, "#1"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account1,
                                            "Account1")
                                    },
                                    new TransactionViewModel
                                    {
                                        Id = BudgetingContextSeed.Transaction2,
                                        Date = new DateTime(2020, 1, 3),
                                        Amount = 20m,
                                        Description = "Transaction2",
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1,
                                            "Category1", CategoryType.Expense, "#1"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account1,
                                            "Account1")
                                    },
                                    new TransactionViewModel
                                    {
                                        Id = BudgetingContextSeed.Transaction3,
                                        Date = new DateTime(2020, 1, 4),
                                        Amount = 30m,
                                        Description = "Transaction3",
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1,
                                            "Category1", CategoryType.Expense, "#1"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account1,
                                            "Account1")
                                    },
                                    new TransactionViewModel
                                    {
                                        Id = BudgetingContextSeed.Transaction4,
                                        Date = new DateTime(2020, 1, 5),
                                        Amount = 40m,
                                        Description = "Transaction4",
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category2,
                                            "Category2", CategoryType.Gain, "#2"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account1,
                                            "Account1")
                                    },
                                    new TransactionViewModel
                                    {
                                        Id = BudgetingContextSeed.Transaction5,
                                        Date = new DateTime(2020, 1, 6),
                                        Amount = 50m,
                                        Description = "Transaction5",
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category2,
                                            "Category2", CategoryType.Gain, "#2"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account1,
                                            "Account1")
                                    },
                                    new TransactionViewModel
                                    {
                                        Id = BudgetingContextSeed.Transaction6,
                                        Date = new DateTime(2020, 1, 7),
                                        Amount = 111m,
                                        Description = "Transaction6",
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1,
                                            "Category1", CategoryType.Expense, "#1"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account1,
                                            "Account1")
                                    }
                                }
                            );
                    }
                });
        }

        [Scenario]
        public void GivenGetTransactions_WithAmountRestrictions(HttpClient client, HttpResponseMessage response)
        {
            "Given GetTransactions query"
                .x(() => client = _factory.CreateDefaultClient());

            "When I call the API for amount < 30"
                .x(async () =>
                {
                    const string request = "/api/v2/transactions/get-transactions?filter=amount:lt:30";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the transactions of the user"
                .x(async () =>
                {
                    var result =
                        await response.Content.ReadFromJsonAsync<QueryResult<TransactionViewModel>>(
                            _jsonSerializerOptions);

                    using (new AssertionScope())
                    {
                        result.Should().NotBeNull();
                        result?.TotalCount.Should().Be(2);
                        result?.Items.Should()
                            .HaveCount(2)
                            .And.BeEquivalentTo(new[]
                                {
                                    new TransactionViewModel
                                    {
                                        Id = BudgetingContextSeed.Transaction1,
                                        Date = new DateTime(2020, 1, 2),
                                        Amount = 10m,
                                        Description = "Transaction1",
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1,
                                            "Category1", CategoryType.Expense, "#1"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account1,
                                            "Account1")
                                    },
                                    new TransactionViewModel
                                    {
                                        Id = BudgetingContextSeed.Transaction2,
                                        Date = new DateTime(2020, 1, 3),
                                        Amount = 20m,
                                        Description = "Transaction2",
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1,
                                            "Category1", CategoryType.Expense, "#1"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account1,
                                            "Account1")
                                    }
                                }
                            );
                    }
                });
        }

        [Scenario]
        public void GivenGetTransactions_WithOffsetAndLimitAndSort(HttpClient client, HttpResponseMessage response)
        {
            "Given GetTransactions query"
                .x(() => client = _factory.CreateDefaultClient());

            "When I call the API with an offset of 2 a limit of 2 and sorted by date"
                .x(async () =>
                {
                    const string request = "/api/v2/transactions/get-transactions?offset=2&limit=2&sort=date";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the transactions of the user"
                .x(async () =>
                {
                    var result =
                        await response.Content.ReadFromJsonAsync<QueryResult<TransactionViewModel>>(
                            _jsonSerializerOptions);

                    using (new AssertionScope())
                    {
                        result.Should().NotBeNull();
                        result?.TotalCount.Should().Be(6);
                        result?.Items.Should()
                            .HaveCount(2)
                            .And.BeEquivalentTo(new[]
                                {
                                    new TransactionViewModel
                                    {
                                        Id = BudgetingContextSeed.Transaction3,
                                        Date = new DateTime(2020, 1, 4),
                                        Amount = 30m,
                                        Description = "Transaction3",
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1,
                                            "Category1", CategoryType.Expense, "#1"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account1,
                                            "Account1")
                                    },
                                    new TransactionViewModel
                                    {
                                        Id = BudgetingContextSeed.Transaction4,
                                        Date = new DateTime(2020, 1, 5),
                                        Amount = 40m,
                                        Description = "Transaction4",
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category2,
                                            "Category2", CategoryType.Gain, "#2"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account1,
                                            "Account1")
                                    }
                                }
                            );
                    }
                });
        }

        [Scenario]
        public void GivenGetTransaction_WithValidId(HttpClient client, Guid transactionId,
            HttpResponseMessage response)
        {
            "Given getting transaction by id #2"
                .x(() =>
                {
                    client = _factory.CreateClient();
                    transactionId = BudgetingContextSeed.Transaction2;
                });

            "For user TestUser #1"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    BudgetingContextSeed.TestUserGuid1.ToString()));

            "When I get call the API"
                .x(async () =>
                {
                    string request = $"/api/v2/transactions/{transactionId.ToString()}";

                    // Act
                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the transaction"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<TransactionViewModel>(_jsonSerializerOptions);

                    result.Should().BeEquivalentTo(
                        new TransactionViewModel
                        {
                            Id = BudgetingContextSeed.Transaction2,
                            Date = new DateTime(2020, 1, 3),
                            Amount = 20m,
                            Description = "Transaction2",
                            Tags = ImmutableArray<string>.Empty,
                            Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1,
                                "Category1", CategoryType.Expense, "#1"),
                            Account = new AccountInfoViewModel(BudgetingContextSeed.Account1,
                                "Account1")
                        }
                    );
                });
        }

        [Scenario]
        public void MakePurchaseCommand(HttpClient client, MakePurchaseCommand command,
            HttpResponseMessage response)
        {
            "Given MakePurchase command"
                .x(() => client = _factory.CreateClient());

            "For newly registered test user"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    Guid.NewGuid().ToString()));

            "With valid data"
                .x(() => command = new MakePurchaseCommand
                {
                    Date = DateTime.Now,
                    Amount = 999.19m,
                    Description = "New purchase",
                    AccountId = BudgetingContextSeed.Account4,
                    CategoryId = BudgetingContextSeed.Category1,
                    Tags = new [] {"Tag1", "Tag2"}
                });

            "When I call the API"
                .x(async () =>
                {
                    const string requestUri = "/api/v2/transactions/make-purchase";

                    response = await client.PostAsJsonAsync(requestUri, command);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "With the header location present"
                .x(() => response.Headers.Location?.AbsolutePath.Should().StartWithEquivalentOf("/api/v2/transactions/"));

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
        public void TransferCommand(HttpClient client, TransferCommand command,
            HttpResponseMessage response)
        {
            "Given TransferCommand command"
                .x(() => client = _factory.CreateClient());

            "For command test user"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    BudgetingContextSeed.TestUserForCommands.ToString()));

            "With valid data"
                .x(() => command = new TransferCommand
                {
                    Date = DateTime.Now,
                    Amount = 100m,
                    Description = "Transfering money",
                    FromAccountId = BudgetingContextSeed.TargetAccount1,
                    ToAccountId = BudgetingContextSeed.TargetAccount2,
                });

            "When I call the API"
                .x(async () =>
                {
                    const string requestUri = "/api/v2/transactions/transfer";

                    response = await client.PostAsJsonAsync(requestUri, command);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "With the header location present"
                .x(() => response.Headers.Location?.AbsolutePath.Should().StartWithEquivalentOf("/api/v2/transactions/"));

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
    }
}
