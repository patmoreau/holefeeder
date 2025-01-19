using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;

using Npgsql;

namespace Holefeeder.Infrastructure.SeedWork;

internal class NpgsqlConnectionManager(BudgetingConnectionStringBuilder connectionStringBuilder)
    : DatabaseConnectionManager(new DelegateConnectionFactory(log =>
    {
        Log = log;
        return new NpgsqlConnection(connectionStringBuilder.ConnectionString);
    }))
{
    public static IUpgradeLog Log { get; private set; } = null!;

    public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents) =>
        PostgresCommandSplitter.SplitCommands(scriptContents);
}
