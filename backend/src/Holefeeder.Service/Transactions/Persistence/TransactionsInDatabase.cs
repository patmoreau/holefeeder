using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core;

using Npgsql;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Transactions.Persistence;

internal sealed partial class TransactionsInDatabase(NpgsqlDataSource dataSource, ILogger<TransactionsInDatabase> logger) : ITransactions
{
    public Task<Result<IReadOnlyList<TransactionAccount>>> GetOpenAccountsAsync(User user, CancellationToken cancellationToken) =>
        QueryAsync<AccountRow, TransactionAccount>(
            """
            SELECT id, name, favorite
            FROM accounts
            WHERE user_id = @UserId AND NOT inactive;
            """,
            user, row => row.ToAccount(), cancellationToken);

    public Task<Result<IReadOnlyList<TransactionCategory>>> GetActiveCategoriesAsync(User user, CancellationToken cancellationToken) =>
        QueryAsync<CategoryRow, TransactionCategory>(
            """
            SELECT id, name, color, type, favorite
            FROM categories
            WHERE user_id = @UserId AND NOT system AND NOT inactive;
            """,
            user, row => row.ToCategory(), cancellationToken);

    public Task<Result<IReadOnlyList<TagUsage>>> GetTagUsagesAsync(User user, CancellationToken cancellationToken) =>
        QueryAsync<TagUsageRow, TagUsage>(
            """
            SELECT LOWER(TRIM(tag)) AS tag, t.category_id, COUNT(*)::INT AS count
            FROM transactions t, UNNEST(STRING_TO_ARRAY(t.tags, ',')) AS tag
            WHERE t.user_id = @UserId AND TRIM(tag) <> ''
            GROUP BY LOWER(TRIM(tag)), t.category_id;
            """,
            user, row => row.ToTagUsage(), cancellationToken);

    public Task<Result<TransactionAccount>> GetOpenAccountAsync(User user, Id id, CancellationToken cancellationToken) =>
        QuerySingleAsync<AccountRow, TransactionAccount>(
            """
            SELECT id, name, favorite
            FROM accounts
            WHERE id = @Id AND user_id = @UserId AND NOT inactive;
            """,
            user, id, row => row.ToAccount(), TransactionErrors.AccountNotFound, cancellationToken);

    public Task<Result<TransactionCategory>> GetActiveCategoryAsync(User user, Id id, CancellationToken cancellationToken) =>
        QuerySingleAsync<CategoryRow, TransactionCategory>(
            """
            SELECT id, name, color, type, favorite
            FROM categories
            WHERE id = @Id AND user_id = @UserId AND NOT system AND NOT inactive;
            """,
            user, id, row => row.ToCategory(), TransactionErrors.CategoryNotFound, cancellationToken);

    public async Task<Result<Nothing>> SaveAsync(
        User user, IReadOnlyList<Transaction> transactions, Cashflow? cashflow, CancellationToken cancellationToken)
    {
        const string saveCashflowSql = """
                                       INSERT INTO cashflows (id, effective_date, amount, interval_type, frequency, recurrence,
                                                              description, account_id, category_id, inactive, tags, user_id)
                                       VALUES (@Id, @EffectiveDate, @Amount, @IntervalType, @Frequency, 0,
                                               @Description, @AccountId, @CategoryId, FALSE, @Tags, @UserId)
                                       ON CONFLICT (id) DO UPDATE
                                       SET effective_date = EXCLUDED.effective_date, amount = EXCLUDED.amount,
                                           interval_type = EXCLUDED.interval_type, frequency = EXCLUDED.frequency,
                                           description = EXCLUDED.description, account_id = EXCLUDED.account_id,
                                           category_id = EXCLUDED.category_id, tags = EXCLUDED.tags
                                       WHERE cashflows.user_id = EXCLUDED.user_id;
                                       """;
        const string saveTransactionSql = """
                                          INSERT INTO transactions (id, date, amount, description, account_id, category_id,
                                                                    cashflow_id, cashflow_date, tags, user_id)
                                          VALUES (@Id, @Date, @Amount, @Description, @AccountId, @CategoryId,
                                                  @CashflowId, @CashflowDate, @Tags, @UserId)
                                          ON CONFLICT (id) DO UPDATE
                                          SET date = EXCLUDED.date, amount = EXCLUDED.amount, description = EXCLUDED.description,
                                              account_id = EXCLUDED.account_id, category_id = EXCLUDED.category_id,
                                              cashflow_id = EXCLUDED.cashflow_id, cashflow_date = EXCLUDED.cashflow_date,
                                              tags = EXCLUDED.tags
                                          WHERE transactions.user_id = EXCLUDED.user_id;
                                          """;

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            await using var dbTransaction = await connection.BeginTransactionAsync(cancellationToken);

            if (cashflow is not null)
            {
                await connection.ExecuteAsync(new CommandDefinition(saveCashflowSql, new
                {
                    Id = cashflow.Id.Value,
                    EffectiveDate = cashflow.EffectiveDate.ToDateTime(TimeOnly.MinValue),
                    Amount = cashflow.Amount.Value,
                    IntervalType = cashflow.IntervalType.Name,
                    Frequency = cashflow.Frequency.Value,
                    cashflow.Description,
                    AccountId = cashflow.AccountId.Value,
                    CategoryId = cashflow.CategoryId.Value,
                    Tags = cashflow.Tags.Text,
                    UserId = user.Id.Value
                }, dbTransaction, cancellationToken: cancellationToken));
            }

            foreach (var transaction in transactions)
            {
                await connection.ExecuteAsync(new CommandDefinition(saveTransactionSql, new
                {
                    Id = transaction.Id.Value,
                    Date = transaction.Date.ToDateTime(TimeOnly.MinValue),
                    Amount = transaction.Amount.Value,
                    transaction.Description,
                    AccountId = transaction.AccountId.Value,
                    CategoryId = transaction.CategoryId.Value,
                    CashflowId = transaction.CashflowId?.Value,
                    CashflowDate = transaction.CashflowDate?.ToDateTime(TimeOnly.MinValue),
                    Tags = transaction.Tags.Text,
                    UserId = user.Id.Value
                }, dbTransaction, cancellationToken: cancellationToken));
            }

            await dbTransaction.CommitAsync(cancellationToken);
            return Nothing.Value;
        }
        catch (NpgsqlException exception)
        {
            LogWriteFailed(logger, exception);
            return TransactionsErrors.WriteFailed;
        }
    }

    private async Task<Result<T>> QuerySingleAsync<TRow, T>(
        string sql, User user, Id id, Func<TRow, T> map, ResultError notFound, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var row = await connection.QuerySingleOrDefaultAsync<TRow>(
                new CommandDefinition(sql, new DynamicParameters(new { Id = id.Value, UserId = user.Id.Value }), cancellationToken: cancellationToken));

            return row is null ? notFound : map(row);
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return TransactionsErrors.ReadFailed;
        }
    }

    private async Task<Result<IReadOnlyList<T>>> QueryAsync<TRow, T>(
        string sql, User user, Func<TRow, T> map, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var rows = await connection.QueryAsync<TRow>(
                new CommandDefinition(sql, new DynamicParameters(new { UserId = user.Id.Value }), cancellationToken: cancellationToken));

            return rows.Select(map).ToList();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return TransactionsErrors.ReadFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Transactions could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Transactions could not be written to the database")]
    private static partial void LogWriteFailed(ILogger logger, Exception exception);
}

internal static class TransactionsErrors
{
    public const string CodeReadFailed = "transactions-read-failed";

    public const string CodeWriteFailed = "transactions-write-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Transactions could not be read from the database.");

    public static UnexpectedError WriteFailed => new(CodeWriteFailed, "Transactions could not be written to the database.");
}
