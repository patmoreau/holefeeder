using System;
using System.Collections.Immutable;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using DrifterApps.Holefeeder.Budgeting.API;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Domain.Extensions;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentAssertions;
using FluentAssertions.Execution;

using Microsoft.AspNetCore.Mvc.Testing;

using Xbehave;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Scenarios
{
    public class CashflowScenarios : IClassFixture<BudgetingWebApplicationFactory>
    {
        private readonly BudgetingWebApplicationFactory _factory;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CashflowScenarios(BudgetingWebApplicationFactory factory)
        {
            _factory = factory;
            
            _factory.SeedData();

            _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [Scenario]
        public void GivenGetCashflows(HttpClient client, HttpResponseMessage response)
        {
            "Given GetCashflows query"
                .x(() => client = _factory.CreateDefaultClient());

            "For user TestUser #2"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    BudgetingContextSeed.TestUserGuid2.ToString()));

            "When I call the API"
                .x(async () =>
                {
                    const string request = "/api/v2/cashflows/get-cashflows";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the cashflows of the user"
                .x(async () =>
                {
                    var result =
                        await response.Content.ReadFromJsonAsync<QueryResult<CashflowViewModel>>(
                            _jsonSerializerOptions);

                    using (new AssertionScope())
                    {
                        result.Should().NotBeNull();
                        result?.TotalCount.Should().Be(2);
                        result?.Items.Should()
                            .HaveCount(2)
                            .And.BeEquivalentTo(new[]
                                {
                                    new CashflowViewModel
                                    {
                                        Id = BudgetingContextSeed.Cashflow3,
                                        EffectiveDate = new DateTime(2020, 1, 4),
                                        Amount = 222m,
                                        Description = "Cashflow3",
                                        Frequency = 2,
                                        IntervalType = DateIntervalType.Weekly,
                                        Recurrence = 0,
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1,
                                            "Category1", CategoryType.Expense, "#1"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account4,
                                            "Account4")
                                    },
                                    new CashflowViewModel
                                    {
                                        Id = BudgetingContextSeed.Cashflow4,
                                        EffectiveDate = new DateTime(2020, 1, 5),
                                        Amount = 333m,
                                        Description = "Cashflow4",
                                        Frequency = 2,
                                        IntervalType = DateIntervalType.OneTime,
                                        Recurrence = 0,
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1,
                                            "Category1", CategoryType.Expense, "#1"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account4,
                                            "Account4")
                                    }
                                }
                            );
                    }
                });
        }

        [Scenario]
        public void GivenGetCashflows_WithAmountRestrictions(HttpClient client, HttpResponseMessage response)
        {
            "Given GetCashflows query"
                .x(() => client = _factory.CreateDefaultClient());

            "For user TestUser #2"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    BudgetingContextSeed.TestUserGuid2.ToString()));

            "When I call the API for amount < 300"
                .x(async () =>
                {
                    const string request = "/api/v2/cashflows/get-cashflows?filter=amount:lt:300";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the cashflows of the user"
                .x(async () =>
                {
                    var result =
                        await response.Content.ReadFromJsonAsync<QueryResult<CashflowViewModel>>(
                            _jsonSerializerOptions);

                    using (new AssertionScope())
                    {
                        result.Should().NotBeNull();
                        result?.TotalCount.Should().Be(1);
                        result?.Items.Should()
                            .HaveCount(1)
                            .And.BeEquivalentTo(new[]
                                {
                                    new CashflowViewModel
                                    {
                                        Id = BudgetingContextSeed.Cashflow3,
                                        EffectiveDate = new DateTime(2020, 1, 4),
                                        Amount = 222m,
                                        Description = "Cashflow3",
                                        Frequency = 2,
                                        IntervalType = DateIntervalType.Weekly,
                                        Recurrence = 0,
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1,
                                            "Category1", CategoryType.Expense, "#1"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account4,
                                            "Account4")
                                    }
                                }
                            );
                    }
                });
        }

        [Scenario]
        public void GivenGetCashflows_WithOffsetAndLimitAndSort(HttpClient client, HttpResponseMessage response)
        {
            "Given GetCashflows query"
                .x(() => client = _factory.CreateDefaultClient());

            "For user TestUser #2"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    BudgetingContextSeed.TestUserGuid2.ToString()));

            "When I call the API with an offset of 0 a limit of 1 and sorted by effective date desc"
                .x(async () =>
                {
                    const string request = "/api/v2/cashflows/get-cashflows?offset=0&limit=1&sort=-effective_date";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the cashflows of the user"
                .x(async () =>
                {
                    var result =
                        await response.Content.ReadFromJsonAsync<QueryResult<CashflowViewModel>>(
                            _jsonSerializerOptions);

                    using (new AssertionScope())
                    {
                        result.Should().NotBeNull();
                        result?.TotalCount.Should().Be(2);
                        result?.Items.Should()
                            .HaveCount(1)
                            .And.BeEquivalentTo(new[]
                                {
                                    new CashflowViewModel
                                    {
                                        Id = BudgetingContextSeed.Cashflow4,
                                        EffectiveDate = new DateTime(2020, 1, 5),
                                        Amount = 333m,
                                        Description = "Cashflow4",
                                        Frequency = 2,
                                        IntervalType = DateIntervalType.OneTime,
                                        Recurrence = 0,
                                        Tags = ImmutableArray<string>.Empty,
                                        Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1,
                                            "Category1", CategoryType.Expense, "#1"),
                                        Account = new AccountInfoViewModel(BudgetingContextSeed.Account4,
                                            "Account4")
                                    }
                                }
                            );
                    }
                });
        }

        [Scenario]
        public void GivenGetCashflow_WithValidId(HttpClient client, Guid cashflowId,
            HttpResponseMessage response)
        {
            "Given getting Cashflow by id #3"
                .x(() =>
                {
                    client = _factory.CreateClient();
                    cashflowId = BudgetingContextSeed.Cashflow3;
                });

            "For user TestUser #2"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    BudgetingContextSeed.TestUserGuid2.ToString()));

            "When I get call the API"
                .x(async () =>
                {
                    string request = $"/api/v2/cashflows/{cashflowId.ToString()}";

                    // Act
                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain the Cashflow"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<CashflowViewModel>(_jsonSerializerOptions);

                    result.Should().BeEquivalentTo(
                        new CashflowViewModel
                        {
                            Id = BudgetingContextSeed.Cashflow3,
                            EffectiveDate = new DateTime(2020, 1, 4),
                            Amount = 222m,
                            Description = "Cashflow3",
                            Frequency = 2,
                            IntervalType = DateIntervalType.Weekly,
                            Recurrence = 0,
                            Tags = ImmutableArray<string>.Empty,
                            Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1,
                                "Category1", CategoryType.Expense, "#1"),
                            Account = new AccountInfoViewModel(BudgetingContextSeed.Account4,
                                "Account4")
                        }
                    );
                });
        }

        [Scenario]
        public void GivenGetUpcoming_WithDateRange(DateTime from, DateTime to, HttpResponseMessage response)
        {
            "Given GetUpcoming query from 2020-01-15"
                .x(() => from = new DateTime(2020, 1, 15));

            "And for up to 2 weeks"
                .x(() => to = from.AddDays(14));

            "When I call the API"
                .x(async () =>
                {
                    var client = _factory.CreateClient();
                    string request =
                        $"/api/v2/cashflows/get-upcoming?from={from.ToPersistent()}&to={to.ToPersistent()}";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());

            "And the result contain a single cashflow"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<UpcomingViewModel[]>(_jsonSerializerOptions);

                    result.Should()
                        .NotBeEmpty()
                        .And.HaveCount(1)
                        .And.ContainEquivalentOf(
                            new UpcomingViewModel
                            {
                                Id = BudgetingContextSeed.Cashflow1,
                                Date = new DateTime(2020, 1, 16),
                                Amount = 111,
                                Description = "Cashflow1",
                                Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1, "Category1",
                                    CategoryType.Expense, "#1"),
                                Account = new AccountInfoViewModel(BudgetingContextSeed.Account1, "Account1")
                            });
                });
        }

        [Scenario]
        public void GivenGetUpcoming_WithUnpaidCashflows(DateTime from, DateTime to, HttpResponseMessage response,
            UpcomingViewModel[] result)
        {
            "Given GetUpcoming query from 2020-01-29"
                .x(() => from = new DateTime(2020, 1, 29));

            "And for up to 2 weeks"
                .x(() => to = from.AddDays(14));

            "When I call the API"
                .x(async () =>
                {
                    var client = _factory.CreateClient();
                    string request =
                        $"/api/v2/cashflows/get-upcoming?from={from.ToPersistent()}&to={to.ToPersistent()}";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());


            "And the result contain 2 cashflows"
                .x(async () =>
                {
                    result = await response.Content.ReadFromJsonAsync<UpcomingViewModel[]>(_jsonSerializerOptions);

                    result.Should()
                        .NotBeEmpty()
                        .And.HaveCount(2);
                });

            "And be in ascending order on the Date"
                .x(() => result.Should().BeInAscendingOrder(x => x.Date));

            "With the unpaid cashflow"
                .x(() => result.Should()
                    .ContainEquivalentOf(
                        new UpcomingViewModel
                        {
                            Id = BudgetingContextSeed.Cashflow1,
                            Date = new DateTime(2020, 1, 16),
                            Amount = 111,
                            Description = "Cashflow1",
                            Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1, "Category1",
                                CategoryType.Expense, "#1"),
                            Account = new AccountInfoViewModel(BudgetingContextSeed.Account1, "Account1")
                        }));

            "And the current period's cashflow"
                .x(() => result.Should()
                    .ContainEquivalentOf(
                        new UpcomingViewModel
                        {
                            Id = BudgetingContextSeed.Cashflow1,
                            Date = new DateTime(2020, 1, 30),
                            Amount = 111,
                            Description = "Cashflow1",
                            Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1, "Category1",
                                CategoryType.Expense, "#1"),
                            Account = new AccountInfoViewModel(BudgetingContextSeed.Account1, "Account1")
                        }));
        }

        [Scenario]
        public void GivenGetUpcoming_ForMultiplePeriods(DateTime from, DateTime to, HttpResponseMessage response,
            UpcomingViewModel[] result)
        {
            "Given GetUpcoming query from 2020-01-15"
                .x(() => from = new DateTime(2020, 1, 15));

            "And for up to 4 weeks"
                .x(() => to = from.AddDays(28));

            "When I call the API"
                .x(async () =>
                {
                    var client = _factory.CreateClient();
                    string request =
                        $"/api/v2/cashflows/get-upcoming?from={from.ToPersistent()}&to={to.ToPersistent()}";

                    response = await client.GetAsync(request);
                });

            "Then the status code should indicate success"
                .x(() => response.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpResponseMessage>()
                    .Which.IsSuccessStatusCode.Should().BeTrue());


            "And the result contain 2 cashflows"
                .x(async () =>
                {
                    result = await response.Content.ReadFromJsonAsync<UpcomingViewModel[]>(_jsonSerializerOptions);

                    result.Should()
                        .NotBeEmpty()
                        .And.HaveCount(2);
                });

            "And be in ascending order on the Date"
                .x(() => result.Should().BeInAscendingOrder(x => x.Date));

            "With the current period's cashflow"
                .x(() => result.Should()
                    .ContainEquivalentOf(
                        new UpcomingViewModel
                        {
                            Id = BudgetingContextSeed.Cashflow1,
                            Date = new DateTime(2020, 1, 16),
                            Amount = 111,
                            Description = "Cashflow1",
                            Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1, "Category1",
                                CategoryType.Expense, "#1"),
                            Account = new AccountInfoViewModel(BudgetingContextSeed.Account1, "Account1")
                        }, o => o.ComparingByValue<CategoryType>()));

            "And the following period's cashflow"
                .x(() => result.Should()
                    .ContainEquivalentOf(
                        new UpcomingViewModel
                        {
                            Id = BudgetingContextSeed.Cashflow1,
                            Date = new DateTime(2020, 1, 30),
                            Amount = 111,
                            Description = "Cashflow1",
                            Category = new CategoryInfoViewModel(BudgetingContextSeed.Category1, "Category1",
                                CategoryType.Expense, "#1"),
                            Account = new AccountInfoViewModel(BudgetingContextSeed.Account1, "Account1")
                        }, options => options.ComparingByMembers<UpcomingViewModel>()));
        }
    }
}
