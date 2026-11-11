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
}

internal static class TransactionsErrors
{
    public const string CodeReadFailed = "transactions-read-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Transactions could not be read from the database.");
}
