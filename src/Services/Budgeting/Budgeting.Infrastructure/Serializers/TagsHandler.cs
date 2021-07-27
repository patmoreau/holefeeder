using System;
using System.Data;

using Dapper;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Serializers
{
    public class TagsHandler : SqlMapper.TypeHandler<string[]>
    {
        public override void SetValue(IDbDataParameter parameter, string[] value)
        {
            parameter.Value = string.Join(";", value);
        }

        public override string[] Parse(object value) => (value as string)?.Split(";") ?? Array.Empty<string>();
    }
}
