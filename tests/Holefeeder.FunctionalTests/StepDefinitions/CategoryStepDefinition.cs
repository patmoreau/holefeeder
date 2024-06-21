using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Scenarios;
using DrifterApps.Seeds.Testing.StepDefinitions;

using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed class CategoryStepDefinition(IHttpClientDriver httpClientDriver, BudgetingDatabaseDriver budgetingDatabaseDriver)
    : StepDefinition(httpClientDriver)
{
    public const string ContextExistingCategory = $"{nameof(CategoryStepDefinition)}_{nameof(ContextExistingCategory)}";

    public void Exists(IStepRunner runner) =>
        runner.Execute("a user has an active category", async () =>
        {
            var category = await GivenACategory().ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(ContextExistingCategory, category);
        });
}
