using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Accounts.Core.GetAccountTransactions;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.Shared.Persistence;

using Npgsql;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Persistence;

internal sealed partial class AccountsInDatabase(NpgsqlDataSource dataSource, ILogger<AccountsInDatabase> logger) : IAccounts
{
    private const string UniqueNamePerUserIndex = "user_id_name_idx";

    public async Task<Result<PeriodSettings>> GetPeriodSettingsAsync(User user, DateOnly today, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT s.data
                           FROM store_items s
                           WHERE s.user_id = @UserId AND s.code = 'settings'
                           LIMIT 1;
                           """;
        var parameters = new DynamicParameters(new { UserId = user.Id.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var data = await connection.QuerySingleOrDefaultAsync<string>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return data is null ? PeriodSettings.Default(today) : SettingsData.ToPeriodSettings(data, AccountsErrors.SettingsInvalid);
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return AccountsErrors.ReadFailed;
        }
    }

    public async Task<Result<AccountTotals>> GetAccountTotalsAsync(User user, Id accountId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT a.id, a.name, a.type, a.description, a.open_balance, a.open_date, a.favorite, a.inactive,
                                  COALESCE(t.gains, 0) AS gains, COALESCE(t.expenses, 0) AS expenses, t.last_transaction_date
                           FROM accounts a
                           LEFT JOIN (SELECT tr.account_id,
                                             SUM(CASE WHEN c.type = 'Gain' THEN tr.amount ELSE 0 END) AS gains,
                                             SUM(CASE WHEN c.type = 'Expense' THEN tr.amount ELSE 0 END) AS expenses,
                                             MAX(tr.date) AS last_transaction_date
                                      FROM transactions tr
                                      INNER JOIN categories c ON c.id = tr.category_id
                                      WHERE tr.user_id = @UserId AND tr.account_id = @AccountId
                                      GROUP BY tr.account_id) t ON t.account_id = a.id
                           WHERE a.user_id = @UserId AND a.id = @AccountId;
                           """;
        var parameters = new DynamicParameters(new { UserId = user.Id.Value, AccountId = accountId.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var row = await connection.QuerySingleOrDefaultAsync<AccountTotalsRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return row is null ? AccountErrors.NotFound : row.ToAccountTotals();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return AccountsErrors.ReadFailed;
        }
    }

    public async Task<Result<IReadOnlyList<CashflowSchedule>>> GetActiveCashflowsAsync(User user, Id accountId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT c.account_id, c.effective_date, c.interval_type, c.frequency, c.amount, cc.type AS category_type,
                                  p.last_paid_date, p.last_cashflow_date
                           FROM cashflows c
                           INNER JOIN categories cc ON cc.id = c.category_id
                           LEFT JOIN (SELECT t.cashflow_id, MAX(t.date) AS last_paid_date, MAX(t.cashflow_date) AS last_cashflow_date
                                      FROM transactions t
                                      WHERE t.user_id = @UserId AND t.cashflow_id IS NOT NULL
                                      GROUP BY t.cashflow_id) p ON p.cashflow_id = c.id
                           WHERE c.user_id = @UserId AND c.account_id = @AccountId AND NOT c.inactive;
                           """;
        var parameters = new DynamicParameters(new { UserId = user.Id.Value, AccountId = accountId.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var rows = await connection.QueryAsync<CashflowScheduleRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return rows.Select(row => row.ToCashflowSchedule()).ToList();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return AccountsErrors.ReadFailed;
        }
    }

    public async Task<Result<PagedData<RecordedTransaction>>> GetTransactionsAsync(
        User user, Id accountId, Offset offset, Limit limit, CancellationToken cancellationToken)
    {
        const string accountSql = "SELECT COUNT(*) FROM accounts WHERE user_id = @UserId AND id = @AccountId;";
        const string countSql = "SELECT COUNT(*) FROM transactions WHERE user_id = @UserId AND account_id = @AccountId;";
        var parameters = new DynamicParameters(new { UserId = user.Id.Value, AccountId = accountId.Value });
        var transactionsSql = $"""
                               SELECT t.id, t.account_id, t.date, t.amount, t.description, c.name AS category_name, c.color AS category_color, t.tags
                               FROM transactions t
                               INNER JOIN categories c ON c.id = t.category_id
                               WHERE t.user_id = @UserId AND t.account_id = @AccountId
                               ORDER BY t.date DESC, t.id DESC
                               {PagingClause.Create(offset, limit, parameters)};
                               """;

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var accounts = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(accountSql, parameters, cancellationToken: cancellationToken));
            if (accounts == 0)
            {
                return AccountErrors.NotFound;
            }

            var total = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
            var rows = await connection.QueryAsync<RecordedTransactionRow>(
                new CommandDefinition(transactionsSql, parameters, cancellationToken: cancellationToken));

            return new PagedData<RecordedTransaction>(total, [.. rows.Select(row => row.ToRecordedTransaction())]);
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return AccountsErrors.ReadFailed;
        }
    }

    public async Task<Result<Nothing>> SaveAsync(User user, Account account, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO accounts (id, type, name, favorite, open_balance, open_date, description, inactive, user_id)
                           VALUES (@Id, @Type, @Name, @Favorite, @OpenBalance, @OpenDate, @Description, @Inactive, @UserId)
                           ON CONFLICT (id) DO UPDATE
                           SET type = EXCLUDED.type, name = EXCLUDED.name, favorite = EXCLUDED.favorite,
                               open_balance = EXCLUDED.open_balance, open_date = EXCLUDED.open_date,
                               description = EXCLUDED.description, inactive = EXCLUDED.inactive
                           WHERE accounts.user_id = EXCLUDED.user_id;
                           """;
        var parameters = new DynamicParameters(new
        {
            Id = account.Id.Value,
            Type = account.Type.Name,
            Name = account.Name.Value,
            account.Favorite,
            OpenBalance = account.OpenBalance.Value,
            OpenDate = account.OpenDate.ToDateTime(TimeOnly.MinValue),
            account.Description,
            account.Inactive,
            UserId = user.Id.Value
        });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var saved = await connection.ExecuteAsync(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return saved == 0 ? AccountErrors.NotFound : Nothing.Value;
        }
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.UniqueViolation &&
                                                  exception.ConstraintName == UniqueNamePerUserIndex)
        {
            return AccountNameErrors.AlreadyExists;
        }
        catch (NpgsqlException exception)
        {
            LogWriteFailed(logger, exception);
            return AccountsErrors.WriteFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Accounts could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Accounts could not be written to the database")]
    private static partial void LogWriteFailed(ILogger logger, Exception exception);
}

internal static class AccountsErrors
{
    public const string CodeReadFailed = "accounts-read-failed";
    public const string CodeSettingsInvalid = "accounts-settings-invalid";
    public const string CodeWriteFailed = "accounts-write-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Accounts could not be read from the database.");

    public static UnexpectedError SettingsInvalid => new(CodeSettingsInvalid, "User settings could not be read.");

    public static UnexpectedError WriteFailed => new(CodeWriteFailed, "Accounts could not be written to the database.");
}
