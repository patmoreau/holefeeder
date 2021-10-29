using System;
using System.Data;
using System.Text;

using DbUp.Engine;

using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Scripts
{
    public class Script005MongoDbMigration : IScript
    {
        public static readonly string ScriptName =
            $"{typeof(Script000InitDatabase).Namespace}.005-MongoDbMigration.sql";

        public string ProvideScript(Func<IDbCommand> dbCommandFactory)
        {
            var scriptBuilder = new StringBuilder();

            scriptBuilder.AppendLine("SELECT COUNT(*) FROM accounts;");
            scriptBuilder.AppendLine("SELECT COUNT(*) FROM categories;");
            scriptBuilder.AppendLine("SELECT COUNT(*) FROM cashflows;");
            scriptBuilder.AppendLine("SELECT COUNT(*) FROM transactions;");

            return scriptBuilder.ToString();
        }
    }
}
