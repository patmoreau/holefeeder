using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Scenarios;
using DrifterApps.Seeds.Testing.StepDefinitions;

using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed class AccountStepDefinition(IHttpClientDriver httpClientDriver, BudgetingDatabaseDriver budgetingDatabaseDriver)
    : StepDefinition(httpClientDriver)
{
    public const string ContextExistingAccount = $"{nameof(AccountStepDefinition)}_{nameof(ContextExistingAccount)}";

    public void Exists(IStepRunner runner) =>
        runner.Execute("a user has an active account", async () =>
        {
            var builder = GivenAnActiveAccount().ForUser(TestUsers[AuthorizedUser].UserId);
            var account = await builder.SavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(ContextExistingAccount, account);
        });
}
