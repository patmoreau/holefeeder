using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed class CategorySteps(BudgetingDatabaseDriver budgetingDatabaseDriver)
{
    public void Exists(IStepRunner runner) =>
        runner.Execute("category exists", async () =>
        {
            var category = await GivenACategory()
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(CategoryContext.ExistingCategory, category);
        });

    public void CollectionExists(IStepRunner runner) =>
        runner.Execute("multiple categories exists", async () =>
        {
            var categories = await GivenACategory()
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .CollectionSavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(CategoryContext.ExistingCategories, categories);
        });

    public void TransferCategoriesExists(IStepRunner runner) =>
        runner.Execute(async () =>
        {
            var categoryIn = await GivenATransferInCategory().ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);
            var categoryOut = await GivenATransferOutCategory().ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            var categories = new List<Category> {categoryIn, categoryOut};
            runner.SetContextData(CategoryContext.ExistingCategories, categories);
        });
}
