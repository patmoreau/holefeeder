using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.SeedWork;
using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Infrastructure.Extensions;

using Microsoft.Extensions.Options;

using MySqlConnector;

using Respawn;

using TechTalk.SpecFlow.Assist;

namespace Holefeeder.FunctionalTests.Drivers;

[Binding]
public class DatabaseDriver
{
    private readonly ObjectStoreDatabaseSettings _settings;

    public DatabaseDriver(IOptions<ObjectStoreDatabaseSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task ResetState()
    {
        var checkpoint = new Checkpoint
        {
            SchemasToInclude = new[] {"object_store_functional_tests"},
            DbAdapter = DbAdapter.MySql,
            TablesToInclude = new Respawn.Graph.Table[] {"store_items"},
            TablesToIgnore = new Respawn.Graph.Table[] {"schema_versions"}
        };

        await using var connection = new MySqlConnection(_settings.ConnectionString);

        await connection.OpenAsync();

        await checkpoint.Reset(connection);
    }

    public async Task AddAccountsToDatabase(Table items)
    {
        await using var connection = new MySqlConnection(_settings.ConnectionString);

        var rows = items.CreateSet(row => new AccountEntity
        {
            Description = row.ContainsKey(nameof(AccountEntity.Description)) ? row[nameof(AccountEntity.Description)] : string.Empty,
            Favorite = row.ContainsKey("Favorite") && Convert.ToBoolean(row["Favorite"]),
            Id = Guid.Parse(row["Id"]),
            Inactive = row.ContainsKey("Inactive") && Convert.ToBoolean(row["Inactive"]),
            Name = row["Name"],
            OpenBalance = row.ContainsKey("OpenBalance") ? Convert.ToDecimal(row["OpenBalance"]) : 0m,
            OpenDate = row.ContainsKey("OpenDate") ? Convert.ToDateTime(row["OpenDate"]) : DateTime.Today,
            Type = Enumeration.FromName<AccountType>(row["Type"]),
            UserId = Guid.Parse(row["UserId"])
        });

        foreach (var row in rows)
        {
            await connection.InsertAsync(row);
        }
    }

    public async Task AddStoreItemsToDatabase(Table items)
    {
        await using var connection = new MySqlConnection(_settings.ConnectionString);

        var rows = items.CreateSet<StoreItemEntity>();

        foreach (var row in rows)
        {
            await connection.InsertAsync(row);
        }
    }
}
