using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.MySql;

using MySqlConnector;

namespace Holefeeder.Infrastructure.SeedWork;

internal class MySqlConnectionManager(BudgetingConnectionStringBuilder connectionStringBuilder) : DatabaseConnectionManager(new DelegateConnectionFactory(log =>
        {
            Log = log;
            return new MySqlConnection(connectionStringBuilder.ConnectionString);
        }))
{
    public static IUpgradeLog Log { get; private set; } = null!;

    public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
    {
        var commandSplitter = new MySqlCommandSplitter();
        IEnumerable<string>? scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
        return scriptStatements;
    }
}
