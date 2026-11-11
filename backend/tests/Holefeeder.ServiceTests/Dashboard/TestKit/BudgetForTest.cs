using Dapper;

using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Dashboard.TestKit;

internal sealed class BudgetForTest(User user, DatabaseForTest database)
{
    private const string InsertAccount =
        """
        INSERT INTO accounts (id, type, name, favorite, open_balance, open_date, description, inactive, user_id)
        VALUES (@Id, 'Checking', @Name, FALSE, 0, '2020-01-01', '', FALSE, @UserId);
        """;

    private const string InsertCategory =
        """
        INSERT INTO categories (id, type, name, color, budget_amount, favorite, system, user_id)
        VALUES (@Id, @Type, @Name, '#000000', 0, FALSE, @System, @UserId);
        """;

    private const string InsertTransaction =
        """
        INSERT INTO transactions (id, date, amount, description, account_id, category_id, tags, user_id)
        VALUES (@Id, @Date, @Amount, '', @AccountId, @CategoryId, '', @UserId);
        """;

    private const string InsertSettings =
        "INSERT INTO store_items (id, code, data, user_id) VALUES (@Id, 'settings', @Data, @UserId);";

    private Guid? _accountId;

    public async Task<Guid> ACategoryAsync(CategoryType type, bool system = false)
    {
        var id = Guid.NewGuid();
        await ExecuteAsync(InsertCategory, new { Id = id, Type = type.ToString(), Name = $"category-{id}", System = system, UserId = user.Id.Value });
        return id;
    }

    public async Task ATransactionAsync(Guid categoryId, DateOnly date, decimal amount)
    {
        var accountId = await AccountAsync();
        await ExecuteAsync(InsertTransaction, new
        {
            Id = Guid.NewGuid(),
            Date = date.ToDateTime(TimeOnly.MinValue),
            Amount = amount,
            AccountId = accountId,
            CategoryId = categoryId,
            UserId = user.Id.Value
        });
    }

    public Task SettingsAsync(string data) =>
        ExecuteAsync(InsertSettings, new { Id = Guid.NewGuid(), Data = data, UserId = user.Id.Value });

    private async Task<Guid> AccountAsync()
    {
        if (_accountId is { } accountId)
        {
            return accountId;
        }

        var id = Guid.NewGuid();
        await ExecuteAsync(InsertAccount, new { Id = id, Name = $"account-{id}", UserId = user.Id.Value });
        _accountId = id;
        return id;
    }

    private async Task ExecuteAsync(string sql, object parameters)
    {
        await using var connection = database.DataSource.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, parameters));
    }
}
