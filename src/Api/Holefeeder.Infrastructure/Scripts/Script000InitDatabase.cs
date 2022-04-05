using System.Data;
using System.Text;

using DbUp.Engine;

using Holefeeder.Infrastructure.Context;

namespace Holefeeder.Infrastructure.Scripts;

public class Script000InitDatabase : IScript
{
    public static readonly string ScriptName =
        $"{typeof(Script000InitDatabase).Namespace}.000-InitDatabase.sql";

    private readonly ObjectStoreDatabaseSettings _objectStoreDatabaseSettings;

    public Script000InitDatabase(ObjectStoreDatabaseSettings objectStoreDatabaseSettings)
    {
        _objectStoreDatabaseSettings = objectStoreDatabaseSettings;
    }

    public string ProvideScript(Func<IDbCommand> dbCommandFactory)
    {
        var builder = _objectStoreDatabaseSettings.GetBuilder();

        var scriptBuilder = new StringBuilder();

        scriptBuilder.AppendLine($"GRANT ALL ON {builder.Database}.* TO '{builder.UserID}'@'%' WITH GRANT OPTION;");

        return scriptBuilder.ToString();
    }
}
