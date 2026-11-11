using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;
using Holefeeder.Service.Dashboard.Core.GetDashboardSummary;
using Holefeeder.Service.Shared.Core;

using Npgsql;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Dashboard.Persistence;

internal sealed partial class DashboardInDatabase(NpgsqlDataSource dataSource, ILogger<DashboardInDatabase> logger)
    : IDashboard
{
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

            return data is null ? PeriodSettings.Default(today) : SettingsData.ToPeriodSettings(data);
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return DashboardErrors.ReadFailed;
        }
    }

    public async Task<Result<IReadOnlyList<DailyTotal>>> GetDailyTotalsAsync(User user, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT c.type AS category_type, t.date, SUM(t.amount) AS total
                           FROM transactions t
                           INNER JOIN categories c ON c.id = t.category_id
                           WHERE c.user_id = @UserId AND NOT c.system
                           GROUP BY c.type, t.date;
                           """;
        var parameters = new DynamicParameters(new { UserId = user.Id.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var rows = await connection.QueryAsync<DailyTotalRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return rows.Select(row => row.ToDailyTotal()).ToList();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return DashboardErrors.ReadFailed;
        }
    }

    public async Task<Result<IReadOnlyList<AccountTotals>>> GetActiveAccountsAsync(User user, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT a.id, a.name, a.type, a.open_balance, a.open_date,
                                  COALESCE(t.gains, 0) AS gains, COALESCE(t.expenses, 0) AS expenses, t.last_transaction_date
                           FROM accounts a
                           LEFT JOIN (SELECT tr.account_id,
                                             SUM(CASE WHEN c.type = 'Gain' THEN tr.amount ELSE 0 END) AS gains,
                                             SUM(CASE WHEN c.type = 'Expense' THEN tr.amount ELSE 0 END) AS expenses,
                                             MAX(tr.date) AS last_transaction_date
                                      FROM transactions tr
                                      INNER JOIN categories c ON c.id = tr.category_id
                                      WHERE tr.user_id = @UserId
                                      GROUP BY tr.account_id) t ON t.account_id = a.id
                           WHERE a.user_id = @UserId AND NOT a.inactive
                           ORDER BY a.favorite DESC, a.name;
                           """;
        var parameters = new DynamicParameters(new { UserId = user.Id.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var rows = await connection.QueryAsync<AccountTotalsRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return rows.Select(row => row.ToAccountTotals()).ToList();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return DashboardErrors.ReadFailed;
        }
    }

    public async Task<Result<IReadOnlyList<Cashflow>>> GetActiveCashflowsAsync(User user, CancellationToken cancellationToken)
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
                           WHERE c.user_id = @UserId AND NOT c.inactive;
                           """;
        var parameters = new DynamicParameters(new { UserId = user.Id.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var rows = await connection.QueryAsync<CashflowRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return rows.Select(row => row.ToCashflow()).ToList();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return DashboardErrors.ReadFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Dashboard could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);
}

internal static class DashboardErrors
{
    public const string CodeReadFailed = "dashboard-read-failed";
    public const string CodeSettingsInvalid = "dashboard-settings-invalid";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Dashboard could not be read from the database.");

    public static UnexpectedError SettingsInvalid => new(CodeSettingsInvalid, "User settings could not be read.");
}
