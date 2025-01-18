using DrifterApps.Seeds.FluentScenario;

using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed class CategorySteps(BudgetingDatabaseDriver budgetingDatabaseDriver)
{
    public void Exists(IStepRunner runner) =>
        runner.Execute("a user has an active category", async () =>
        {
            var category = await GivenACategory().ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(CategoryContexts.ExistingCategory, category);
        });
}
