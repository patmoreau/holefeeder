using System.Data;

using Dapper;

namespace Holefeeder.Infrastructure.Serializers;

internal class TagsHandler : SqlMapper.TypeHandler<string[]>
{
    public override void SetValue(IDbDataParameter parameter, string[] value)
    {
        parameter.Value = string.Join(";", value);
    }

    public override string[] Parse(object value)
    {
        return (value as string)?.Split(";") ?? Array.Empty<string>();
    }
}
