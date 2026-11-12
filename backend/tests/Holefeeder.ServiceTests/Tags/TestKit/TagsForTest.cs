using Dapper;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Tags.TestKit;

internal static class TagsForTest
{
    private const string InsertAccountAndCategory =
        """
        INSERT INTO accounts (id, type, name, favorite, open_balance, open_date, description, inactive, user_id)
        VALUES (@AccountId, 'Checking', @AccountName, FALSE, 0, '2020-01-01', '', FALSE, @UserId);
        INSERT INTO categories (id, type, name, color, budget_amount, favorite, system, user_id)
        VALUES (@CategoryId, 'Expense', @CategoryName, '#000000', 0, FALSE, FALSE, @UserId);
        """;

    private const string InsertTransaction =
        """
        INSERT INTO transactions (id, date, amount, description, account_id, category_id, tags, user_id)
        VALUES (@Id, '2026-11-01', 10, '', @AccountId, @CategoryId, @Tags, @UserId);
        """;

    public static async Task<Id> TaggedTransactionInDbAsync(this DatabaseForTest database, User user, string? tags)
    {
        var id = AnId();
        var accountId = AnId().Value;
        var categoryId = AnId().Value;
        await using var connection = database.DataSource.CreateConnection();
        await connection.ExecuteAsync(InsertAccountAndCategory, new
        {
            AccountId = accountId,
            AccountName = $"account-{accountId}",
            CategoryId = categoryId,
            CategoryName = $"category-{categoryId}",
            UserId = user.Id.Value
        });
        await connection.ExecuteAsync(InsertTransaction, new
        {
            Id = id.Value,
            AccountId = accountId,
            CategoryId = categoryId,
            Tags = tags,
            UserId = user.Id.Value
        });
        return id;
    }

    public static async Task<Id> TaggedCashflowInDbAsync(this DatabaseForTest database, User user, string tags, bool inactive = false) =>
        (await ACashflow(tags: tags, inactive: inactive).SavedInDbAsync(user, database)).Id;

    public static async Task<string?> TagsOfAsync(this DatabaseForTest database, string table, Id id)
    {
        await using var connection = database.DataSource.CreateConnection();
        return await connection.QuerySingleAsync<string?>($"SELECT tags FROM {table} WHERE id = @Id", new { Id = id.Value });
    }
}
