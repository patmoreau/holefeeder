using Dapper;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.TestKit.Persistence;

internal sealed class BudgetForTest(User user, DatabaseForTest database)
{
    private const string InsertAccount =
        """
        INSERT INTO accounts (id, type, name, favorite, open_balance, open_date, description, inactive, user_id)
        VALUES (@Id, @Type, @Name, @Favorite, @OpenBalance, @OpenDate, @Description, @Inactive, @UserId);
        """;

    private const string InsertCategory =
        """
        INSERT INTO categories (id, type, name, color, budget_amount, favorite, system, inactive, user_id)
        VALUES (@Id, @Type, @Name, @Color, 0, @Favorite, @System, @Inactive, @UserId);
        """;

    private const string InsertTransaction =
        """
        INSERT INTO transactions (id, date, amount, description, account_id, category_id, tags, user_id)
        VALUES (@Id, @Date, @Amount, @Description, @AccountId, @CategoryId, @Tags, @UserId);
        """;

    private const string InsertCashflow =
        """
        INSERT INTO cashflows (id, effective_date, amount, interval_type, frequency, recurrence, description, account_id, category_id, inactive, tags, user_id)
        VALUES (@Id, @EffectiveDate, @Amount, @IntervalType, @Frequency, 0, @Description, @AccountId, @CategoryId, @Inactive, @Tags, @UserId);
        """;

    private const string InsertPayment =
        """
        INSERT INTO transactions (id, date, amount, description, account_id, category_id, cashflow_id, cashflow_date, tags, user_id)
        VALUES (@Id, @Date, @Amount, '', @AccountId, @CategoryId, @CashflowId, @CashflowDate, '', @UserId);
        """;

    private const string InsertSettings =
        "INSERT INTO store_items (id, code, data, user_id) VALUES (@Id, 'settings', @Data, @UserId);";

    private Guid? _accountId;

    public async Task<Guid> ACategoryAsync(
        CategoryType type, bool system = false, string? name = null, string color = "#000000", bool favorite = false, bool inactive = false)
    {
        var id = Guid.NewGuid();
        await ExecuteAsync(InsertCategory, new
        {
            Id = id,
            Type = type.ToString(),
            Name = name ?? $"category-{id}",
            Color = color,
            Favorite = favorite,
            System = system,
            Inactive = inactive,
            UserId = user.Id.Value
        });
        return id;
    }

    public async Task<Guid> AnAccountAsync(
        string name, AccountType type, decimal openBalance = 0m, DateOnly? openDate = null, bool favorite = false, bool inactive = false,
        string description = "")
    {
        var id = Guid.NewGuid();
        await ExecuteAsync(InsertAccount, new
        {
            Id = id,
            Type = type.Name,
            Name = name,
            Favorite = favorite,
            OpenBalance = openBalance,
            OpenDate = (openDate ?? new DateOnly(2020, 1, 1)).ToDateTime(TimeOnly.MinValue),
            Description = description,
            Inactive = inactive,
            UserId = user.Id.Value
        });
        return id;
    }

    public async Task<Guid> ATransactionAsync(
        Guid categoryId, DateOnly date, decimal amount, Guid? accountId = null, string description = "", string? tags = "")
    {
        var id = Guid.NewGuid();
        accountId ??= await TheAccountAsync();
        await ExecuteAsync(InsertTransaction, new
        {
            Id = id,
            Date = date.ToDateTime(TimeOnly.MinValue),
            Amount = amount,
            Description = description,
            AccountId = accountId,
            CategoryId = categoryId,
            Tags = tags,
            UserId = user.Id.Value
        });
        return id;
    }

    public async Task<Guid> ACashflowAsync(
        Guid categoryId,
        DateOnly effectiveDate,
        IntervalType intervalType,
        int frequency,
        decimal amount,
        bool inactive = false,
        Guid? accountId = null,
        string description = "",
        string? tags = "")
    {
        var id = Guid.NewGuid();
        await ExecuteAsync(InsertCashflow, new
        {
            Id = id,
            EffectiveDate = effectiveDate.ToDateTime(TimeOnly.MinValue),
            Amount = amount,
            IntervalType = intervalType.Name,
            Frequency = frequency,
            Description = description,
            AccountId = accountId ?? await TheAccountAsync(),
            CategoryId = categoryId,
            Inactive = inactive,
            Tags = tags,
            UserId = user.Id.Value
        });
        return id;
    }

    public async Task APaymentAsync(Guid cashflowId, Guid categoryId, DateOnly date, DateOnly cashflowDate, decimal amount) =>
        await ExecuteAsync(InsertPayment, new
        {
            Id = Guid.NewGuid(),
            Date = date.ToDateTime(TimeOnly.MinValue),
            Amount = amount,
            AccountId = await TheAccountAsync(),
            CategoryId = categoryId,
            CashflowId = cashflowId,
            CashflowDate = cashflowDate.ToDateTime(TimeOnly.MinValue),
            UserId = user.Id.Value
        });

    public Task SettingsAsync(string data) =>
        ExecuteAsync(InsertSettings, new { Id = Guid.NewGuid(), Data = data, UserId = user.Id.Value });

    public async Task<Guid> TheAccountAsync()
    {
        if (_accountId is { } accountId)
        {
            return accountId;
        }

        var id = await AnAccountAsync($"account-{Guid.NewGuid()}", AccountType.Checking);
        _accountId = id;
        return id;
    }

    private async Task ExecuteAsync(string sql, object parameters)
    {
        await using var connection = database.DataSource.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, parameters));
    }
}
