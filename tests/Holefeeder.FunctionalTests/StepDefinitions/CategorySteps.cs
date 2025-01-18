using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Domain.Features.Categories;
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

            runner.SetContextData(CategoryContext.ExistingCategory, category);
        });

    public void TransferCategoriesExists(IStepRunner runner) =>
        runner.Execute("transfer categories exists", async () =>
        {
            var categoryIn = await GivenATransferInCategory().ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);
            var categoryOut = await GivenATransferOutCategory().ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            var categories = new List<Category> {categoryIn, categoryOut};
            runner.SetContextData(CategoryContext.ExistingCategories, categories);
        });
}
