using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Cashflows.Core.ListCashflows;
using Holefeeder.Service.Shared.Core;

using Npgsql;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Cashflows.Persistence;

internal sealed partial class CashflowsInDatabase(NpgsqlDataSource dataSource, ILogger<CashflowsInDatabase> logger) : ICashflows
{
    public async Task<Result<IReadOnlyList<CashflowSummary>>> ListAsync(User user, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT c.id, c.description, c.category_id, c.account_id, c.effective_date, c.interval_type, c.frequency, c.amount,
                                  cc.type AS category_type, p.last_paid_date, p.last_cashflow_date, cc.name AS category_name,
                                  cc.color AS category_color, a.name AS account_name, c.tags
                           FROM cashflows c
                           INNER JOIN categories cc ON cc.id = c.category_id
                           INNER JOIN accounts a ON a.id = c.account_id
                           LEFT JOIN (SELECT t.cashflow_id, MAX(t.date) AS last_paid_date, MAX(t.cashflow_date) AS last_cashflow_date
                                      FROM transactions t
                                      WHERE t.user_id = @UserId AND t.cashflow_id IS NOT NULL
                                      GROUP BY t.cashflow_id) p ON p.cashflow_id = c.id
                           WHERE c.user_id = @UserId AND NOT c.inactive
                           ORDER BY cc.type, LOWER(COALESCE(NULLIF(TRIM(c.description), ''), cc.name)), c.id;
                           """;
        var parameters = new DynamicParameters(new { UserId = user.Id.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var rows = await connection.QueryAsync<CashflowSummaryRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return rows.Select(row => row.ToCashflowSummary()).ToList();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return CashflowsErrors.ReadFailed;
        }
    }

    public async Task<Result<Cashflow>> GetAsync(User user, Id id, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT id, effective_date, amount, interval_type, frequency, recurrence, description,
                                  account_id, category_id, inactive, tags
                           FROM cashflows
                           WHERE id = @Id AND user_id = @UserId;
                           """;
        var parameters = new DynamicParameters(new
        {
            Id = id.Value,
            UserId = user.Id.Value
        });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var row = await connection.QuerySingleOrDefaultAsync<CashflowRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return row is null ? CashflowErrors.NotFound : row.ToCashflow();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return CashflowsErrors.ReadFailed;
        }
    }

    public async Task<Result<Nothing>> SaveAsync(User user, Cashflow cashflow, Payment? payment, CancellationToken cancellationToken)
    {
        const string saveCashflowSql = """
                                       INSERT INTO cashflows (id, effective_date, amount, interval_type, frequency, recurrence,
                                                              description, account_id, category_id, inactive, tags, user_id)
                                       VALUES (@Id, @EffectiveDate, @Amount, @IntervalType, @Frequency, @Recurrence,
                                               @Description, @AccountId, @CategoryId, @Inactive, @Tags, @UserId)
                                       ON CONFLICT (id) DO UPDATE
                                       SET effective_date = EXCLUDED.effective_date, amount = EXCLUDED.amount,
                                           interval_type = EXCLUDED.interval_type, frequency = EXCLUDED.frequency,
                                           recurrence = EXCLUDED.recurrence, description = EXCLUDED.description,
                                           account_id = EXCLUDED.account_id, category_id = EXCLUDED.category_id,
                                           inactive = EXCLUDED.inactive, tags = EXCLUDED.tags
                                       WHERE cashflows.user_id = EXCLUDED.user_id;
                                       """;
        const string insertPaymentSql = """
                                        INSERT INTO transactions (id, date, amount, description, account_id, category_id,
                                                                  cashflow_id, cashflow_date, tags, user_id)
                                        VALUES (@Id, @Date, @Amount, @Description, @AccountId, @CategoryId,
                                                @CashflowId, @CashflowDate, @Tags, @UserId);
                                        """;

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            var saved = await connection.ExecuteAsync(new CommandDefinition(saveCashflowSql, new
            {
                Id = cashflow.Id.Value,
                EffectiveDate = cashflow.EffectiveDate.ToDateTime(TimeOnly.MinValue),
                Amount = cashflow.Amount.Value,
                cashflow.IntervalType,
                cashflow.Frequency,
                cashflow.Recurrence,
                cashflow.Description,
                AccountId = cashflow.AccountId.Value,
                CategoryId = cashflow.CategoryId.Value,
                cashflow.Inactive,
                cashflow.Tags,
                UserId = user.Id.Value
            }, transaction, cancellationToken: cancellationToken));
            if (saved == 0)
            {
                return CashflowErrors.NotFound;
            }

            if (payment is not null)
            {
                await connection.ExecuteAsync(new CommandDefinition(insertPaymentSql, new
                {
                    Id = payment.Id.Value,
                    Date = payment.Date.ToDateTime(TimeOnly.MinValue),
                    Amount = payment.Amount.Value,
                    payment.Description,
                    AccountId = payment.AccountId.Value,
                    CategoryId = payment.CategoryId.Value,
                    CashflowId = payment.CashflowId.Value,
                    CashflowDate = payment.CashflowDate.ToDateTime(TimeOnly.MinValue),
                    payment.Tags,
                    UserId = user.Id.Value
                }, transaction, cancellationToken: cancellationToken));
            }

            await transaction.CommitAsync(cancellationToken);
            return Nothing.Value;
        }
        catch (NpgsqlException exception)
        {
            LogWriteFailed(logger, exception);
            return CashflowsErrors.WriteFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Cashflows could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Cashflows could not be written to the database")]
    private static partial void LogWriteFailed(ILogger logger, Exception exception);
}

internal static class CashflowsErrors
{
    public const string CodeReadFailed = "cashflows-read-failed";
    public const string CodeWriteFailed = "cashflows-write-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Cashflows could not be read from the database.");

    public static UnexpectedError WriteFailed => new(CodeWriteFailed, "Cashflows could not be written to the database.");
}
