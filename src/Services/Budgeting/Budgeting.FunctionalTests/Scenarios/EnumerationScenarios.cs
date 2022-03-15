using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using FluentAssertions;

using Xbehave;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Scenarios;

public class EnumerationScenarios : IClassFixture<BudgetingWebApplicationFactory>
{
    private readonly BudgetingWebApplicationFactory _factory;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public EnumerationScenarios(BudgetingWebApplicationFactory factory)
    {
        _factory = factory;

        _factory.SeedData();

        _jsonSerializerOptions = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
    }

    [Scenario]
    public void GivenGetAccountTypes(HttpClient client, HttpResponseMessage response)
    {
        "Given GetAccountTypes query"
            .x(() => client = _factory.CreateDefaultClient());

        "When I call the API"
            .x(async () =>
            {
                const string request = "/api/v2/enumerations/get-account-types";

                response = await client.GetAsync(request);
            });

        "Then the status code should indicate success"
            .x(() => response.Should()
                .NotBeNull()
                .And.BeOfType<HttpResponseMessage>()
                .Which.IsSuccessStatusCode.Should().BeTrue());

        "And the result contain the account types of the API"
            .x(async () =>
            {
                var result = await response.Content.ReadFromJsonAsync<string[]>(_jsonSerializerOptions);

                result.Should()
                    .NotBeEmpty()
                    .And.HaveCount(7)
                    .And.BeEquivalentTo("Checking", "CreditCard", "CreditLine", "Investment", "Loan", "Mortgage",
                        "Savings");
            });
    }

    [Scenario]
    public void GivenGetCategoryTypes(HttpClient client, HttpResponseMessage response)
    {
        "Given GetAccountTypes query"
            .x(() => client = _factory.CreateDefaultClient());

        "When I call the API"
            .x(async () =>
            {
                const string request = "/api/v2/enumerations/get-category-types";

                response = await client.GetAsync(request);
            });

        "Then the status code should indicate success"
            .x(() => response.Should()
                .NotBeNull()
                .And.BeOfType<HttpResponseMessage>()
                .Which.IsSuccessStatusCode.Should().BeTrue());

        "And the result contain the category types of the API"
            .x(async () =>
            {
                var result = await response.Content.ReadFromJsonAsync<string[]>(_jsonSerializerOptions);

                result.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2)
                    .And.BeEquivalentTo("Expense", "Gain");
            });
    }

    [Scenario]
    public void GivenGetDateIntervalTypes(HttpClient client, HttpResponseMessage response)
    {
        "Given GetAccountTypes query"
            .x(() => client = _factory.CreateDefaultClient());

        "When I call the API"
            .x(async () =>
            {
                const string request = "/api/v2/enumerations/get-date-interval-types";

                response = await client.GetAsync(request);
            });

        "Then the status code should indicate success"
            .x(() => response.Should()
                .NotBeNull()
                .And.BeOfType<HttpResponseMessage>()
                .Which.IsSuccessStatusCode.Should().BeTrue());

        "And the result contain the date interval types of the API"
            .x(async () =>
            {
                var result = await response.Content.ReadFromJsonAsync<string[]>(_jsonSerializerOptions);

                result.Should()
                    .NotBeEmpty()
                    .And.HaveCount(4)
                    .And.BeEquivalentTo("Weekly", "Monthly", "Yearly", "OneTime");
            });
    }
}
