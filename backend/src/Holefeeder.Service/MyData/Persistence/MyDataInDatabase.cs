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

    [LoggerMessage(Level = LogLevel.Error, Message = "My data could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);
}

internal static class MyDataDatabaseErrors
{
    public const string CodeReadFailed = "my-data-read-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "My data could not be read from the database.");
}
