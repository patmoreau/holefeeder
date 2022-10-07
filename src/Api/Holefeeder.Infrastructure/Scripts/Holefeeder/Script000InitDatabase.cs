using System.Data;
using System.Globalization;
using System.Text;

using DbUp.Engine;

using Holefeeder.Infrastructure.Context;

namespace Holefeeder.Infrastructure.Scripts.Holefeeder;

public class Script000InitDatabase : IScript
{
    public static readonly string ScriptName =
        $"{typeof(Script000InitDatabase).Namespace}.000-InitDatabase.sql";

    private readonly HolefeederDatabaseSettings _holefeederDatabaseSettings;

    public Script000InitDatabase(HolefeederDatabaseSettings holefeederDatabaseSettings)
    {
        _holefeederDatabaseSettings = holefeederDatabaseSettings;
    }

    public string ProvideScript(Func<IDbCommand> dbCommandFactory)
    {
        var builder = _holefeederDatabaseSettings.GetBuilder();

        var scriptBuilder = new StringBuilder();

        scriptBuilder.AppendLine(CultureInfo.InvariantCulture,
            $"GRANT ALL ON {builder.Database}.* TO '{builder.UserID}'@'%' WITH GRANT OPTION;");

        return scriptBuilder.ToString();
    }
}
