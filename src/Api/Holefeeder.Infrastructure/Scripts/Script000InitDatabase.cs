using System.Data;
using System.Globalization;
using System.Text;

using DbUp.Engine;

using Holefeeder.Infrastructure.SeedWork;

using MySqlConnector;

namespace Holefeeder.Infrastructure.Scripts;

internal class Script000InitDatabase : IScript
{
    public static readonly string ScriptName =
        $"{typeof(Script000InitDatabase).Namespace}.000-InitDatabase.sql";

    private readonly MySqlConnectionStringBuilder _connectionStringBuilder;

    public Script000InitDatabase(BudgetingConnectionStringBuilder connectionStringBuilder)
    {
        _connectionStringBuilder = connectionStringBuilder.CreateBuilder();
    }

    public string ProvideScript(Func<IDbCommand> dbCommandFactory)
    {
        var scriptBuilder = new StringBuilder();

        scriptBuilder.AppendLine(CultureInfo.InvariantCulture,
            $"GRANT ALL ON {_connectionStringBuilder.Database}.* TO '{_connectionStringBuilder.UserID}'@'%' WITH GRANT OPTION;");

        return scriptBuilder.ToString();
    }
}
