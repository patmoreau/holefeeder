using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using FluentAssertions;

using Xbehave;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Scenarios
{
    public class CategoryScenarios : IClassFixture<BudgetingWebApplicationFactory>
    {
        private readonly BudgetingWebApplicationFactory _factory;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CategoryScenarios(BudgetingWebApplicationFactory factory)
        {
            _factory = factory;

            _factory.SeedData();

            _jsonSerializerOptions = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
        }

        [Scenario]
        public void GivenGetCategories(HttpClient client, IEnumerable<CategoryViewModel>? result)
        {
            "Given GetCategories query"
                .x(() => client = _factory.CreateDefaultClient());

            "When I call the API"
                .x(async () =>
                {
                    const string request = "/api/v2/categories";

                    result = await client.GetFromJsonAsync<IEnumerable<CategoryViewModel>>(request);
                });

            "And the result contain the categories of the user"
                .x(() => result.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2)
                    .And.ContainInOrder(
                        new CategoryViewModel
                        {
                            Id = BudgetingContextSeed.Category1,
                            Name = "Category1",
                            Color = "#1",
                            BudgetAmount = 0,
                            Favorite = false
                        },
                        new CategoryViewModel
                        {
                            Id = BudgetingContextSeed.Category2,
                            Name = "Category2",
                            Color = "#2",
                            BudgetAmount = 0,
                            Favorite = true
                        }));
        }
    }
}
