using System.Data;

using Dapper;

using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Infrastructure.Serializers;

public class CategoryTypeHandler : SqlMapper.TypeHandler<CategoryType>
{
    public override void SetValue(IDbDataParameter parameter, CategoryType value)
    {
        parameter.Value = value?.Name;
    }

    public override CategoryType Parse(object value)
    {
        return Enumeration.FromName<CategoryType>(value as string ?? string.Empty);
    }
}
