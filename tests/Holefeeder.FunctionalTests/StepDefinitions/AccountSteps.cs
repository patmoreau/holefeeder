using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed partial class AccountSteps(BudgetingDatabaseDriver budgetingDatabaseDriver)
{
    public void Exists(IStepRunner runner) =>
        runner.Execute("a user has an active account", async () =>
        {
            var builder = GivenAnActiveAccount().ForUser(TestUsers[AuthorizedUser].UserId);

            var account = await builder.SavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(AccountContexts.ExistingAccount, account);

            return account;
        });

    public void CollectionExists(IStepRunner runner) =>
        runner.Execute("a user has multiple active accounts", async () =>
        {
            var builder = GivenAnActiveAccount().ForUser(TestUsers[AuthorizedUser].UserId);

            var accounts = await builder.CollectionSavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(AccountContexts.ExistingAccounts, accounts);

            return accounts;
        });

    public void ExistsWithExpenses(IStepRunner runner) =>
        runner.Execute(async () =>
        {
            var account = await GivenAnActiveAccount()
                .OfType(AccountType.Checking)
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            var category = await GivenACategory()
                .OfType(CategoryType.Expense)
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            var transaction = await GivenATransaction()
                .ForAccount(account)
                .ForCategory(category)
                .SavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(AccountContexts.ExistingAccount, account);
            runner.SetContextData(CategoryContexts.ExistingCategory, category);
            runner.SetContextData(TransactionContexts.ExistingTransaction, transaction);
            return (Guid)account.Id;
        });

    public void ExistsWithGains(IStepRunner runner) =>
        runner.Execute(async () =>
        {
            var account = await GivenAnActiveAccount()
                .OfType(AccountType.Checking)
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            var category = await GivenACategory()
                .OfType(CategoryType.Gain)
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            var transaction = await GivenATransaction()
                .ForAccount(account)
                .ForCategory(category)
                .SavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(AccountContexts.ExistingAccount, account);
            runner.SetContextData(CategoryContexts.ExistingCategory, category);
            runner.SetContextData(TransactionContexts.ExistingTransaction, transaction);
            return (Guid)account.Id;
        });
}
