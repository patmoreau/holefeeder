using System.Data;

using Dapper;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Infrastructure.Serializers;

public class DateIntervalTypeHandler : SqlMapper.TypeHandler<DateIntervalType>
{
    public override void SetValue(IDbDataParameter parameter, DateIntervalType value)
    {
        parameter.Value = value?.Name;
    }

    public override DateIntervalType Parse(object value)
    {
        return Enumeration.FromName<DateIntervalType>(value as string ?? string.Empty);
    }
}
