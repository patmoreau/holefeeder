using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Insights.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.Insights.TestKit;

internal sealed class InsightsInMemory : IInsights
{
    private readonly List<(CategoryType Type, InsightCategory Category)> _categories = [];
    private readonly List<(CategoryType Type, InsightEntry Entry)> _entries = [];
    private PeriodSettings? _settings;
    private ResultError? _failure;

    public Task<Result<PeriodSettings>> GetPeriodSettingsAsync(User user, DateOnly today, CancellationToken cancellationToken) =>
        Task.FromResult<Result<PeriodSettings>>(_failure is not null ? _failure : _settings ?? PeriodSettings.Default(today));

    public Task<Result<IReadOnlyList<InsightCategory>>> GetCategoriesAsync(User user, CategoryType type, CancellationToken cancellationToken) =>
        Task.FromResult<Result<IReadOnlyList<InsightCategory>>>(
            _failure is not null ? _failure : _categories.Where(entry => entry.Type == type).Select(entry => entry.Category).ToList());

    public Task<Result<IReadOnlyList<InsightEntry>>> GetEntriesAsync(User user, CategoryType type, CancellationToken cancellationToken) =>
        Task.FromResult<Result<IReadOnlyList<InsightEntry>>>(
            _failure is not null ? _failure : _entries.Where(entry => entry.Type == type).Select(entry => entry.Entry).ToList());

    public void Use(PeriodSettings settings) => _settings = settings;

    public InsightCategory Add(CategoryType type, string name, decimal budget = 0m, string color = "#5856D6")
    {
        var category = new InsightCategory(AnId(), name, color, budget);
        _categories.Add((type, category));
        return category;
    }

    public void Spend(InsightCategory category, DateOnly date, decimal amount, string tags = "") =>
        _entries.Add((CategoryType.Expense, new InsightEntry(date, category.Id, amount, tags)));

    public void Earn(InsightCategory category, DateOnly date, decimal amount, string tags = "") =>
        _entries.Add((CategoryType.Gain, new InsightEntry(date, category.Id, amount, tags)));

    public void FailWith(ResultError error) => _failure = error;
}
