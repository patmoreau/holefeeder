using Dapper;

using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.StoreItems.TestKit;

internal static class StoreItemForTest
{
    private const string Insert = "INSERT INTO store_items (id, code, data, user_id) VALUES (@Id, @Code, @Data, @UserId);";

    internal static StoreItem AStoreItem(Code? code = null, string? data = null) =>
        StoreItem.Valid(AnId(), code ?? ACode(), data ?? "data-value");

    public static async Task<StoreItem> SavedInDbAsync(this StoreItem storeItem, User user, DatabaseForTest database)
    {
        var parameters = new DynamicParameters(new
        {
            Id = storeItem.Id.Value,
            Code = storeItem.Code.Value,
            Data = storeItem.Data,
            UserId = user.Id.Value
        });

        await using var connection = database.DataSource.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(Insert, parameters));
        return storeItem;
    }
}
