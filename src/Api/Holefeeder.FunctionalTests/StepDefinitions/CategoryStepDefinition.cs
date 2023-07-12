using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Scenarios;
using DrifterApps.Seeds.Testing.StepDefinitions;
using Holefeeder.FunctionalTests.Drivers;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class CategoryStepDefinition : StepDefinition
{
    private readonly BudgetingDatabaseDriver _budgetingDatabaseDriver;
    public const string ContextExistingCategory = $"{nameof(CategoryStepDefinition)}_{nameof(ContextExistingCategory)}";

    public CategoryStepDefinition(IHttpClientDriver httpClientDriver, BudgetingDatabaseDriver budgetingDatabaseDriver) : base(httpClientDriver)
    {
        _budgetingDatabaseDriver = budgetingDatabaseDriver;
    }

    internal void Exists(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);

        runner.Execute("a user has an active category", async () =>
        {
            var category = await GivenACategory().ForUser(UserStepDefinition.HolefeederUserId)
                .SavedInDbAsync(_budgetingDatabaseDriver);

            runner.SetContextData(ContextExistingCategory, category);
        });
    }
}
