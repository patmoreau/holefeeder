using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Scenarios;
using DrifterApps.Seeds.Testing.StepDefinitions;
using Holefeeder.FunctionalTests.Drivers;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class AccountStepDefinition : StepDefinition
{
    private readonly BudgetingDatabaseDriver _budgetingDatabaseDriver;
    public const string ContextExistingAccount = $"{nameof(AccountStepDefinition)}_{nameof(ContextExistingAccount)}";

    public AccountStepDefinition(IHttpClientDriver httpClientDriver, BudgetingDatabaseDriver budgetingDatabaseDriver) : base(httpClientDriver)
    {
        _budgetingDatabaseDriver = budgetingDatabaseDriver;
    }

    internal void Exists(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);

        runner.Execute("a user has an active account", async () =>
        {
            var account = await GivenAnActiveAccount().ForUser(UserStepDefinition.HolefeederUserId)
                .SavedInDbAsync(_budgetingDatabaseDriver);

            runner.SetContextData(ContextExistingAccount, account);
        });
    }
}
