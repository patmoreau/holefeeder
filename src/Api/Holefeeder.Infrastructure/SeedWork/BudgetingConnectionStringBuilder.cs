using MySqlConnector;

namespace Holefeeder.Infrastructure.SeedWork;

internal class BudgetingConnectionStringBuilder
{
    public required string ConnectionString { get; init; }

    public MySqlConnectionStringBuilder CreateBuilder() => new(ConnectionString);

    public MySqlConnectionStringBuilder CreateBuilder(MySqlGuidFormat guidFormat) =>
        new(ConnectionString) {GuidFormat = guidFormat};
}
