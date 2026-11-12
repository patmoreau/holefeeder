using System.Data.Common;

using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.Shared.Core;

using Npgsql;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.MyData.Persistence;

internal sealed partial class MyDataInDatabase(NpgsqlDataSource dataSource, ILogger<MyDataInDatabase> logger) : IMyDataStore
{
    private const string SelectAll =
        """
        SELECT id, type, name, open_balance, open_date, description, favorite, inactive
        FROM accounts WHERE user_id = @UserId;
        SELECT id, type, name, color, budget_amount, favorite, system, inactive
        FROM categories WHERE user_id = @UserId;
        SELECT id, effective_date, amount, interval_type, frequency, recurrence, description, tags, category_id, account_id, inactive
        FROM cashflows WHERE user_id = @UserId;
        SELECT id, date, amount, description, tags, category_id, account_id, cashflow_id, cashflow_date
        FROM transactions WHERE user_id = @UserId;
        """;

    public async Task<Result<MyDataSet>> ExportAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            await using var reader = await connection.QueryMultipleAsync(
                new CommandDefinition(SelectAll, new { UserId = user.Id.Value }, cancellationToken: cancellationToken));

            var accounts = (await reader.ReadAsync<MyDataAccountRow>()).Select(row => row.ToRecord()).ToList();
            var categories = (await reader.ReadAsync<MyDataCategoryRow>()).Select(row => row.ToRecord()).ToList();
            var cashflows = (await reader.ReadAsync<MyDataCashflowRow>()).Select(row => row.ToRecord()).ToList();
            var transactions = (await reader.ReadAsync<MyDataTransactionRow>()).Select(row => row.ToRecord()).ToList();

            return new MyDataSet(accounts, categories, cashflows, transactions);
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return MyDataDatabaseErrors.ReadFailed;
        }
    }

    public async Task<Result<IMyDataImport>> BeginImportAsync(User user, CancellationToken cancellationToken)
    {
        NpgsqlConnection? connection = null;
        try
        {
            connection = await dataSource.OpenConnectionAsync(cancellationToken);
            var transaction = await connection.BeginTransactionAsync(cancellationToken);
            return new MyDataImportInDatabase(connection, transaction, user, logger);
        }
        catch (NpgsqlException exception)
        {
            if (connection is not null)
            {
                await connection.DisposeAsync();
            }

            LogBeginFailed(logger, exception);
            return MyDataDatabaseErrors.WriteFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "My data could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "My data import could not start a database transaction")]
    private static partial void LogBeginFailed(ILogger logger, Exception exception);
}

internal sealed partial class MyDataImportInDatabase(
    NpgsqlConnection connection, DbTransaction transaction, User user, ILogger logger) : IMyDataImport
{
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

    public async Task<Result<bool>> ExistsAsync(MyDataTable table, Id id, CancellationToken cancellationToken)
    {
        var sql = $"SELECT EXISTS (SELECT 1 FROM {TableName(table)} WHERE id = @Id AND user_id = @UserId)";
        try
        {
            return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { Id = id.Value, UserId = user.Id.Value },
                transaction, cancellationToken: cancellationToken));
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return MyDataDatabaseErrors.ReadFailed;
        }
    }

    public Task<Result<Nothing>> SaveAsync(MyDataAccount account, CancellationToken cancellationToken) =>
        ExecuteAsync(UpsertAccount, new
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
        }, MyDataTable.Accounts, cancellationToken);

    public Task<Result<Nothing>> SaveAsync(MyDataCategory category, CancellationToken cancellationToken) =>
        ExecuteAsync(UpsertCategory, new
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
        }, MyDataTable.Categories, cancellationToken);

    public Task<Result<Nothing>> SaveAsync(MyDataCashflow cashflow, CancellationToken cancellationToken) =>
        ExecuteAsync(UpsertCashflow, new
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
        }, MyDataTable.Cashflows, cancellationToken);

    public Task<Result<Nothing>> SaveAsync(MyDataTransaction transactionRecord, CancellationToken cancellationToken) =>
        ExecuteAsync(UpsertTransaction, new
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
        }, MyDataTable.Transactions, cancellationToken);

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
            return MyDataDatabaseErrors.WriteFailed;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await transaction.DisposeAsync();
        await connection.DisposeAsync();
    }

    private async Task<Result<Nothing>> ExecuteAsync(string sql, object parameters, MyDataTable table, CancellationToken cancellationToken)
    {
        try
        {
            var affected = await connection.ExecuteAsync(
                new CommandDefinition(sql, parameters, transaction, cancellationToken: cancellationToken));
            return affected == 0 ? MyDataImportErrors.IdTaken(table) : Nothing.Value;
        }
        catch (NpgsqlException exception)
        {
            LogWriteFailed(logger, exception);
            return MyDataDatabaseErrors.WriteFailed;
        }
    }

    private static string TableName(MyDataTable table) =>
        table switch
        {
            MyDataTable.Accounts => "accounts",
            MyDataTable.Categories => "categories",
            MyDataTable.Cashflows => "cashflows",
            _ => "transactions"
        };

    [LoggerMessage(Level = LogLevel.Error, Message = "My data import could not read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "My data import could not write to the database")]
    private static partial void LogWriteFailed(ILogger logger, Exception exception);
}

internal static class MyDataDatabaseErrors
{
    public const string CodeReadFailed = "my-data-read-failed";
    public const string CodeWriteFailed = "my-data-write-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "My data could not be read from the database.");
    public static UnexpectedError WriteFailed => new(CodeWriteFailed, "My data could not be written to the database.");
}
