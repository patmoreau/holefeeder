using Bogus;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed class DataSteps(BudgetingDatabaseDriver budgetingDatabaseDriver)
{
    public void TheUserHasFullSetsOfData(IStepRunner runner) =>
        runner.Execute(async () =>
        {
            var accounts = await GivenAnActiveAccount()
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .CollectionSavedInDbAsync(budgetingDatabaseDriver, new Faker().Random.Int(2, 10));
            runner.SetContextData(AccountContext.ExistingAccounts, accounts);

            var categories = await GivenACategory()
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .CollectionSavedInDbAsync(budgetingDatabaseDriver, new Faker().Random.Int(2, 10));
            runner.SetContextData(CategoryContext.ExistingCategory, categories);

            var cashflows = await GivenAnActiveCashflow()
                .ForAccount(accounts.ElementAt(0))
                .ForCategory(categories.ElementAt(0))
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .CollectionSavedInDbAsync(budgetingDatabaseDriver, new Faker().Random.Int(2, 10));
            runner.SetContextData(CashflowContext.ExistingCashflows, cashflows);

            var transactions = await GivenATransaction()
                .ForAccount(accounts.ElementAt(0))
                .ForCategory(categories.ElementAt(0))
                .CollectionSavedInDbAsync(budgetingDatabaseDriver, new Faker().Random.Int(2, 10));
            runner.SetContextData(TransactionContext.ExistingTransactions, transactions);
        });
}
