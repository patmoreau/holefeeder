using System.Collections.Generic;

using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.MySql;

using MySqlConnector;

namespace Framework.Dapper.SeedWork
{
    public class MySqlConnectionManager : DatabaseConnectionManager
    {
        public static IUpgradeLog Log { get; private set; } = null!;

        public MySqlConnectionManager(string connectionString)
            : base(new DelegateConnectionFactory(log =>
            {
                Log = log;
                return new MySqlConnection(connectionString);
            }))
        {
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var commandSplitter = new MySqlCommandSplitter();
            var scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
            return scriptStatements;
        }
    }
}
