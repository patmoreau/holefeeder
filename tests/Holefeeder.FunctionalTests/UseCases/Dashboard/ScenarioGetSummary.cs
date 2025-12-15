using System.Globalization;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.UseCases.Dashboard;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Features;
using Holefeeder.Tests.Common.Builders.Accounts;
using Holefeeder.Tests.Common.Builders.Categories;
using Holefeeder.Tests.Common.Builders.Transactions;
using Holefeeder.Tests.Common.Builders.Users;

using Refit;

namespace Holefeeder.FunctionalTests.UseCases.Dashboard;

public class ScenarioGetSummary(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    private static readonly DateOnly TestAsOfDate = DateOnly.FromDateTime(DateTime.Now);
    private static readonly DateOnly PreviousMonthDate = TestAsOfDate.AddMonths(-1);
    private static readonly DateOnly OldestMonthDate = TestAsOfDate.AddMonths(-2);
    private readonly UserId _userId = TestUsers[AuthorizedUser].UserId;

    private readonly Dictionary<string, Category> _categories = [];
    private readonly Dictionary<string, Account> _accounts = [];

    [Fact]
    public Task WhenGettingSummaryStatistics() =>
        ScenarioRunner.Create("A user wants to get their current summary", ScenarioOutput)
            .Given(TheUserHasSettingsConfigured)
            .And($"a 'purchase' transaction was made on the 'credit card' account in {TestAsOfDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture)}", () => CreateTransaction("purchase", "credit card", new DateOnly(TestAsOfDate.Year, TestAsOfDate.Month, 1), Money.Create(500.5m).Value))
            .And($"a 'food and drink' transaction was made on the 'credit card' account in {OldestMonthDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture)}", () => CreateTransaction("food and drink", "credit card", new DateOnly(OldestMonthDate.Year, OldestMonthDate.Month, 1), Money.Create(100.1m).Value))
            .And($"a 'food and drink' transaction was made on the 'credit card' account in {PreviousMonthDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture)}", () => CreateTransaction("food and drink", "credit card", new DateOnly(PreviousMonthDate.Year, PreviousMonthDate.Month, 1), Money.Create(200.2m).Value))
            .And($"a 'food and drink' transaction was made on the 'credit card' account in {TestAsOfDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture)}", () => CreateTransaction("food and drink", "credit card", new DateOnly(TestAsOfDate.Year, TestAsOfDate.Month, 1), Money.Create(300.3m).Value))
            .And($"a 'transfer out' transaction that should be ignored in {TestAsOfDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture)}", () => CreateTransaction("transfer out", "credit card", new DateOnly(TestAsOfDate.Year, TestAsOfDate.Month, 1), Money.Create(800.8m).Value, true))
            .And($"a second 'food and drink' transaction was made on the 'checking' account in {TestAsOfDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture)}", () => CreateTransaction("food and drink", "checking", new DateOnly(TestAsOfDate.Year, TestAsOfDate.Month, 10), Money.Create(400.4m).Value))
            .And($"an 'income' gain was made to the 'checking' account in {OldestMonthDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture)}", () => CreateGain("income", "checking", new DateOnly(OldestMonthDate.Year, OldestMonthDate.Month, 1), Money.Create(1000.1m).Value))
            .And($"an 'income' gain was made to the 'checking' account in {PreviousMonthDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture)}", () => CreateGain("income", "checking", new DateOnly(PreviousMonthDate.Year, PreviousMonthDate.Month, 1), Money.Create(2000.2m).Value))
            .And($"an 'income' gain was made to the 'checking' account in {TestAsOfDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture)}", () => CreateGain("income", "checking", new DateOnly(TestAsOfDate.Year, TestAsOfDate.Month, 1), Money.Create(3000.3m).Value))
            .And($"an 'transfer in' transaction that should be ignored in {TestAsOfDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture)}", () => CreateGain("transfer in", "checking", new DateOnly(TestAsOfDate.Year, TestAsOfDate.Month, 1), Money.Create(7000.7m).Value, true))
            .When(TheUser.GetDashboardSummary)
            .Then(TheSummaryShouldMatchTheExpected)
            .PlayAsync();

    private void TheUserHasSettingsConfigured(IStepRunner runner)
    {
        var userSettings = UserSettingsBuilder
            .GivenAUserSettings()
            .WithEffectiveDate(new DateOnly(DateTime.Now.Year - 1, 1, 1))
            .WithIntervalType(DateIntervalType.Monthly)
            .Build();
        StoreItem.HasUserSettings(runner, userSettings);
    }

    private static void TheSummaryShouldMatchTheExpected(IStepRunner runner) =>
        runner.Execute<IApiResponse<SummaryResult>>(response =>
        {
            var expectedSummary = new SummaryResult(1201.2m, 700.7m, 140m, 1799.1m, 3000.3m, 500.5m);

            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveContent();
            var results = response.Value.Content;
            results.Should()
                .NotBeNull()
                .And.BeEquivalentTo(expectedSummary);
        });

    private async Task CreateTransaction(string categoryName, string accountName, DateOnly date, Money amount, bool systemCategory = false)
    {
        if (!_categories.TryGetValue(categoryName, out var category))
        {
            category = await CategoryBuilder
                .GivenACategory()
                .OfType(CategoryType.Expense)
                .WithName(categoryName)
                .ForUser(_userId)
                .IsSystem(systemCategory)
                .SavedInDbAsync(DatabaseDriver);
            _categories.Add(categoryName, category);
        }

        if (!_accounts.TryGetValue(accountName, out var account))
        {
            account = await AccountBuilder.GivenAnActiveAccount().WithName(accountName).ForUser(_userId).SavedInDbAsync(DatabaseDriver);
            _accounts.Add(accountName, account);
        }

        await TransactionBuilder
            .GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .OnDate(date)
            .OfAmount(amount)
            .SavedInDbAsync(DatabaseDriver);
    }

    private async Task CreateGain(string categoryName, string accountName, DateOnly date, Money amount, bool systemCategory = false)
    {
        if (!_categories.TryGetValue(categoryName, out var category))
        {
            category = await CategoryBuilder
                .GivenACategory()
                .OfType(CategoryType.Gain)
                .WithName(categoryName)
                .ForUser(_userId)
                .IsSystem(systemCategory)
                .SavedInDbAsync(DatabaseDriver);
            _categories.Add(categoryName, category);
        }

        if (!_accounts.TryGetValue(accountName, out var account))
        {
            account = await AccountBuilder.GivenAnActiveAccount().WithName(accountName).ForUser(_userId).SavedInDbAsync(DatabaseDriver);
            _accounts.Add(accountName, account);
        }

        await TransactionBuilder
            .GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .OnDate(date)
            .OfAmount(amount)
            .SavedInDbAsync(DatabaseDriver);
    }
}
