using System.Data.Common;

using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Sync.Core;

using Npgsql;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Sync.Persistence;

internal sealed partial class SyncInDatabase(NpgsqlDataSource dataSource, ILogger<SyncInDatabase> logger) : ISyncStore
{
    public async Task<Result<ISyncSession>> BeginAsync(User user, CancellationToken cancellationToken)
    {
        NpgsqlConnection? connection = null;
        try
        {
            connection = await dataSource.OpenConnectionAsync(cancellationToken);
            var transaction = await connection.BeginTransactionAsync(cancellationToken);
            return new SyncSessionInDatabase(connection, transaction, user, logger);
        }
        catch (NpgsqlException exception)
        {
            if (connection is not null)
            {
                await connection.DisposeAsync();
            }

            LogBeginFailed(logger, exception);
            return SyncDatabaseErrors.WriteFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Sync could not start a database transaction")]
    private static partial void LogBeginFailed(ILogger logger, Exception exception);
}

internal sealed partial class SyncSessionInDatabase(
    NpgsqlConnection connection, DbTransaction transaction, User user, ILogger logger) : ISyncSession
{
    private const string SelectAccount =
        """
        SELECT id, type, name, open_balance, open_date, description, favorite, inactive
        FROM accounts WHERE id = @Id AND user_id = @UserId
        """;

    private const string SelectCategory =
        """
        SELECT id, type, name, color, budget_amount, favorite, system, inactive
        FROM categories WHERE id = @Id AND user_id = @UserId
        """;

    private const string SelectStoreItem = "SELECT id, code, data FROM store_items WHERE id = @Id AND user_id = @UserId";

    private const string SelectTransaction =
        """
        SELECT id, date, amount, description, account_id, category_id, cashflow_id, cashflow_date, tags
        FROM transactions WHERE id = @Id AND user_id = @UserId
        """;

    private const string SelectCashflow =
        """
        SELECT id, effective_date, interval_type, frequency, recurrence, amount, description, account_id, category_id, inactive, tags
        FROM cashflows WHERE id = @Id AND user_id = @UserId
        """;

    private const string UpsertAccount =
        """
        INSERT INTO accounts (id, type, name, favorite, open_balance, open_date, description, inactive, user_id)
        VALUES (@Id, @Type, @Name, @Favorite, @OpenBalance, @OpenDate, @Description, @Inactive, @UserId)
        ON CONFLICT (id) DO UPDATE
        SET type = EXCLUDED.type, name = EXCLUDED.name, favorite = EXCLUDED.favorite, open_balance = EXCLUDED.open_balance,
            open_date = EXCLUDED.open_date, description = EXCLUDED.description, inactive = EXCLUDED.inactive
        WHERE accounts.user_id = EXCLUDED.user_id;
        """;

    private const string UpsertCategory =
        """
        INSERT INTO categories (id, type, name, color, budget_amount, favorite, system, inactive, user_id)
        VALUES (@Id, @Type, @Name, @Color, @BudgetAmount, @Favorite, @System, @Inactive, @UserId)
        ON CONFLICT (id) DO UPDATE
        SET type = EXCLUDED.type, name = EXCLUDED.name, color = EXCLUDED.color, budget_amount = EXCLUDED.budget_amount,
            favorite = EXCLUDED.favorite, system = EXCLUDED.system, inactive = EXCLUDED.inactive
        WHERE categories.user_id = EXCLUDED.user_id;
        """;

    private const string UpsertStoreItem =
        """
        INSERT INTO store_items (id, code, data, user_id) VALUES (@Id, @Code, @Data, @UserId)
        ON CONFLICT (id) DO UPDATE SET code = EXCLUDED.code, data = EXCLUDED.data
        WHERE store_items.user_id = EXCLUDED.user_id;
        """;

    private const string UpsertTransaction =
        """
        INSERT INTO transactions (id, date, amount, description, account_id, category_id, cashflow_id, cashflow_date, tags, user_id)
        VALUES (@Id, @Date, @Amount, @Description, @AccountId, @CategoryId, @CashflowId, @CashflowDate, @Tags, @UserId)
        ON CONFLICT (id) DO UPDATE
        SET date = EXCLUDED.date, amount = EXCLUDED.amount, description = EXCLUDED.description, account_id = EXCLUDED.account_id,
            category_id = EXCLUDED.category_id, cashflow_id = EXCLUDED.cashflow_id, cashflow_date = EXCLUDED.cashflow_date,
            tags = EXCLUDED.tags
        WHERE transactions.user_id = EXCLUDED.user_id;
        """;

    private const string UpsertCashflow =
        """
        INSERT INTO cashflows (id, effective_date, amount, interval_type, frequency, recurrence, description, account_id, category_id,
                               inactive, tags, user_id)
        VALUES (@Id, @EffectiveDate, @Amount, @IntervalType, @Frequency, @Recurrence, @Description, @AccountId, @CategoryId,
                @Inactive, @Tags, @UserId)
        ON CONFLICT (id) DO UPDATE
        SET effective_date = EXCLUDED.effective_date, amount = EXCLUDED.amount, interval_type = EXCLUDED.interval_type,
            frequency = EXCLUDED.frequency, recurrence = EXCLUDED.recurrence, description = EXCLUDED.description,
            account_id = EXCLUDED.account_id, category_id = EXCLUDED.category_id, inactive = EXCLUDED.inactive, tags = EXCLUDED.tags
        WHERE cashflows.user_id = EXCLUDED.user_id;
        """;

    public async Task<Result<ISyncRecord>> FindAsync(SyncTable table, Id id, CancellationToken cancellationToken)
    {
        var parameters = new { Id = id.Value, UserId = user.Id.Value };
        try
        {
            ISyncRecord? record = table switch
            {
                SyncTable.Accounts => (await QueryAsync<SyncAccountRow>(SelectAccount, parameters, cancellationToken))?.ToRecord(),
                SyncTable.Categories => (await QueryAsync<SyncCategoryRow>(SelectCategory, parameters, cancellationToken))?.ToRecord(),
                SyncTable.StoreItems => (await QueryAsync<SyncStoreItemRow>(SelectStoreItem, parameters, cancellationToken))?.ToRecord(),
                SyncTable.Transactions =>
                    (await QueryAsync<SyncTransactionRow>(SelectTransaction, parameters, cancellationToken))?.ToRecord(),
                _ => (await QueryAsync<SyncCashflowRow>(SelectCashflow, parameters, cancellationToken))?.ToRecord()
            };

            return record is null ? SyncRecordErrors.NotFound(table) : record.ToResult();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return SyncDatabaseErrors.ReadFailed;
        }
    }

    public async Task<Result<Nothing>> SaveAsync(ISyncRecord record, CancellationToken cancellationToken)
    {
        var (sql, parameters) = record.Table switch
        {
            SyncTable.Accounts => (UpsertAccount, ParametersOf((SyncAccount)record)),
            SyncTable.Categories => (UpsertCategory, ParametersOf((SyncCategory)record)),
            SyncTable.StoreItems => (UpsertStoreItem, ParametersOf((SyncStoreItem)record)),
            SyncTable.Transactions => (UpsertTransaction, ParametersOf((SyncTransaction)record)),
            _ => (UpsertCashflow, ParametersOf((SyncCashflow)record))
        };

        return await ExecuteAsync(sql, parameters, record.Table, cancellationToken);
    }

    public Task<Result<Nothing>> DeleteAsync(SyncTable table, Id id, CancellationToken cancellationToken)
    {
        var sql = $"DELETE FROM {TableName(table)} WHERE id = @Id AND user_id = @UserId";
        return ExecuteAsync(sql, new { Id = id.Value, UserId = user.Id.Value }, table, cancellationToken);
    }

    public async Task<Result<Nothing>> CommitAsync(CancellationToken cancellationToken)
    {
        try
        {
            await transaction.CommitAsync(cancellationToken);
            return Nothing.Value;
        }
        catch (NpgsqlException exception)
        {
            LogWriteFailed(logger, exception);
            return SyncDatabaseErrors.WriteFailed;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await transaction.DisposeAsync();
        await connection.DisposeAsync();
    }

    private Task<TRow?> QueryAsync<TRow>(string sql, object parameters, CancellationToken cancellationToken) =>
        connection.QuerySingleOrDefaultAsync<TRow>(new CommandDefinition(sql, parameters, transaction, cancellationToken: cancellationToken));

    private async Task<Result<Nothing>> ExecuteAsync(string sql, object parameters, SyncTable table, CancellationToken cancellationToken)
    {
        try
        {
            var affected = await connection.ExecuteAsync(
                new CommandDefinition(sql, parameters, transaction, cancellationToken: cancellationToken));
            return affected == 0 ? SyncRecordErrors.NotFound(table) : Nothing.Value;
        }
        catch (NpgsqlException exception)
        {
            LogWriteFailed(logger, exception);
            return SyncDatabaseErrors.WriteFailed;
        }
    }

    private static string TableName(SyncTable table) =>
        table switch
        {
            SyncTable.Accounts => "accounts",
            SyncTable.Categories => "categories",
            SyncTable.StoreItems => "store_items",
            SyncTable.Transactions => "transactions",
            _ => "cashflows"
        };

    private object ParametersOf(SyncAccount account) => new
    {
        Id = account.Id.Value,
        Type = account.Type.Name,
        account.Name,
        account.Favorite,
        OpenBalance = account.OpenBalance.Value,
        OpenDate = account.OpenDate.ToDateTime(TimeOnly.MinValue),
        account.Description,
        account.Inactive,
        UserId = user.Id.Value
    };

    private object ParametersOf(SyncCategory category) => new
    {
        Id = category.Id.Value,
        Type = category.Type.ToString(),
        category.Name,
        category.Color,
        BudgetAmount = category.BudgetAmount.Value,
        category.Favorite,
        category.System,
        category.Inactive,
        UserId = user.Id.Value
    };

    private object ParametersOf(SyncStoreItem item) => new { Id = item.Id.Value, item.Code, item.Data, UserId = user.Id.Value };

    private object ParametersOf(SyncTransaction transactionRecord) => new
    {
        Id = transactionRecord.Id.Value,
        Date = transactionRecord.Date.ToDateTime(TimeOnly.MinValue),
        Amount = transactionRecord.Amount.Value,
        transactionRecord.Description,
        AccountId = transactionRecord.AccountId.Value,
        CategoryId = transactionRecord.CategoryId.Value,
        CashflowId = transactionRecord.CashflowId?.Value,
        CashflowDate = transactionRecord.CashflowDate?.ToDateTime(TimeOnly.MinValue),
        Tags = transactionRecord.Tags.Text,
        UserId = user.Id.Value
    };

    private object ParametersOf(SyncCashflow cashflow) => new
    {
        Id = cashflow.Id.Value,
        EffectiveDate = cashflow.EffectiveDate.ToDateTime(TimeOnly.MinValue),
        Amount = cashflow.Amount.Value,
        IntervalType = cashflow.IntervalType.Name,
        cashflow.Frequency,
        cashflow.Recurrence,
        cashflow.Description,
        AccountId = cashflow.AccountId.Value,
        CategoryId = cashflow.CategoryId.Value,
        cashflow.Inactive,
        Tags = cashflow.Tags.Text,
        UserId = user.Id.Value
    };

    [LoggerMessage(Level = LogLevel.Error, Message = "Sync could not read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Sync could not write to the database")]
    private static partial void LogWriteFailed(ILogger logger, Exception exception);
}

internal static class SyncDatabaseErrors
{
    public const string CodeReadFailed = "sync-read-failed";
    public const string CodeWriteFailed = "sync-write-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Sync data could not be read from the database.");
    public static UnexpectedError WriteFailed => new(CodeWriteFailed, "Sync data could not be written to the database.");
}
