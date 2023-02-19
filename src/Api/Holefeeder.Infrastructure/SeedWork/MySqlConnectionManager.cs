using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.MySql;

using MySqlConnector;

namespace Holefeeder.Infrastructure.SeedWork;

internal class MySqlConnectionManager : DatabaseConnectionManager
{
    public MySqlConnectionManager(BudgetingConnectionStringBuilder connectionStringBuilder)
        : base(new DelegateConnectionFactory(log =>
        {
            Log = log;
            return new MySqlConnection(connectionStringBuilder.ConnectionString);
        }))
    {
    }

    public static IUpgradeLog Log { get; private set; } = null!;

    public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
    {
        var commandSplitter = new MySqlCommandSplitter();
        var scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
        return scriptStatements;
    }
}
