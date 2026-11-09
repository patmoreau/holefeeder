using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Persistence;
using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.GetStoreItem;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;

using Npgsql;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.StoreItems.Persistence;

internal sealed partial class StoreItemsInDatabase(NpgsqlDataSource dataSource, ILogger<StoreItemsInDatabase> logger)
    : IStoreItems
{
    public async Task<Result<StoreItemsPage>> SearchAsync(SearchStoreItemsRequest request, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters(new
        {
            UserId = request.User.Id.Value
        });

        var whereClause = SearchStoreItemsRequestWhereClause.Create(request.FilterBy, request.User, parameters);
        var orderByClause = SearchStoreItemsRequestOrderByClause.Create(request.SortBy);
        var pagingClause = PagingClause.Create(request.Offset, request.Limit, parameters);

        var countSql = $"""
                        SELECT COUNT(*)
                        FROM store_items s
                        INNER JOIN users u ON u.id = s.user_id
                        {whereClause};
                        """;
        var itemsSql = $"""
                         SELECT s.id, s.code, s.data
                         FROM store_items s
                         INNER JOIN users u ON u.id = s.user_id
                         {whereClause}
                         {orderByClause}
                         {pagingClause};
                        """;

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var total = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
            var rows = await connection.QueryAsync<StoreItemRow>(
                new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

            return new StoreItemsPage(total, [.. rows.Select(row => row.ToStoreItem())]);
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return StoreItemsErrors.ReadFailed;
        }
    }

    public async Task<Result<StoreItem>> GetAsync(GetStoreItemRequest request, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT s.id, s.code, s.data
                           FROM store_items s
                           WHERE s.id = @Id AND s.user_id = @UserId;
                           """;
        var parameters = new DynamicParameters(new
        {
            Id = request.Id.Value,
            UserId = request.User.Id.Value
        });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var row = await connection.QuerySingleOrDefaultAsync<StoreItemRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return row is null ? StoreItemErrors.NotFound : row.ToStoreItem();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return StoreItemsErrors.ReadFailed;
        }
    }

    public async Task<Result<Nothing>> SaveAsync(User user, StoreItem item, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO store_items (id, code, data, user_id)
                           VALUES (@Id, @Code, @Data, @UserId)
                           ON CONFLICT (id) DO UPDATE
                           SET code = EXCLUDED.code, data = EXCLUDED.data
                           WHERE store_items.user_id = EXCLUDED.user_id;
                           """;
        var parameters = new DynamicParameters(new
        {
            Id = item.Id.Value,
            Code = item.Code.Value,
            item.Data,
            UserId = user.Id.Value
        });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var saved = await connection.ExecuteAsync(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return saved == 0 ? StoreItemErrors.NotFound : Nothing.Value;
        }
        catch (NpgsqlException exception)
        {
            LogWriteFailed(logger, exception);
            return StoreItemsErrors.WriteFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Store items could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Store items could not be written to the database")]
    private static partial void LogWriteFailed(ILogger logger, Exception exception);
}

internal static class StoreItemsErrors
{
    public const string CodeReadFailed = "store-items-read-failed";
    public const string CodeWriteFailed = "store-items-write-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Store items could not be read from the database.");

    public static UnexpectedError WriteFailed => new(CodeWriteFailed, "Store items could not be written to the database.");
}
