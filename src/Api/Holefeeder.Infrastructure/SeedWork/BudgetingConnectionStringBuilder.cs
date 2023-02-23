using MySqlConnector;

namespace Holefeeder.Infrastructure.SeedWork;

public class BudgetingConnectionStringBuilder
{
    public const string BUDGETING_CONNECTION_STRING = "BudgetingConnectionString";
    public required string ConnectionString { get; init; }

    public MySqlConnectionStringBuilder CreateBuilder() => new(ConnectionString);

    public MySqlConnectionStringBuilder CreateBuilder(MySqlGuidFormat guidFormat) =>
        new(ConnectionString) { GuidFormat = guidFormat };
}
