using Dapper;

using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Transactions.TestKit;

internal static class TransactionsDatabaseForTest
{
    public static async Task<IReadOnlyList<TransactionInDb>> TransactionsInDbAsync(this DatabaseForTest database, Guid accountId)
    {
        await using var connection = database.DataSource.CreateConnection();
        var rows = await connection.QueryAsync<TransactionInDb>(
            """
            SELECT id, date, amount, description, account_id, category_id, cashflow_id, cashflow_date, tags, user_id
            FROM transactions WHERE account_id = @AccountId ORDER BY amount;
            """,
            new { AccountId = accountId });
        return [.. rows];
    }

    public static async Task<IReadOnlyList<CashflowInDb>> CashflowsInDbAsync(this DatabaseForTest database, Guid accountId)
    {
        await using var connection = database.DataSource.CreateConnection();
        var rows = await connection.QueryAsync<CashflowInDb>(
            """
            SELECT id, effective_date, amount, interval_type, frequency, recurrence, description, account_id, category_id, inactive, tags, user_id
            FROM cashflows WHERE account_id = @AccountId;
            """,
            new { AccountId = accountId });
        return [.. rows];
    }
}

internal sealed record TransactionInDb(
    Guid Id, DateOnly Date, decimal Amount, string? Description, Guid AccountId, Guid CategoryId, Guid? CashflowId,
    DateOnly? CashflowDate, string? Tags, Guid UserId);

internal sealed record CashflowInDb(
    Guid Id, DateOnly EffectiveDate, decimal Amount, string IntervalType, int Frequency, int Recurrence, string? Description,
    Guid AccountId, Guid CategoryId, bool Inactive, string? Tags, Guid UserId);
