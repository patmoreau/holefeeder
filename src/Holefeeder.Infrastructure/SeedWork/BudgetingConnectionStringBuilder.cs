using MySqlConnector;

namespace Holefeeder.Infrastructure.SeedWork;

public class BudgetingConnectionStringBuilder
{
    public const string BudgetingConnectionString = "BudgetingConnectionString";
    public required string ConnectionString { get; init; }

    public MySqlConnectionStringBuilder CreateBuilder() => new(ConnectionString);
}
