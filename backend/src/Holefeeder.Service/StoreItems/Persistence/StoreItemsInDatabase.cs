using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Persistence;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;

using Npgsql;

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

    [LoggerMessage(Level = LogLevel.Error, Message = "Store items could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);
}

internal static class StoreItemsErrors
{
    public const string CodeReadFailed = "store-items-read-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Store items could not be read from the database.");
}
