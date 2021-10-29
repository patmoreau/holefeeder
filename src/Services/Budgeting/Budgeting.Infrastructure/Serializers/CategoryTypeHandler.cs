using System.Data;

using Dapper;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Serializers
{
    public class CategoryTypeHandler : SqlMapper.TypeHandler<CategoryType>
    {
        public override void SetValue(IDbDataParameter parameter, CategoryType value)
        {
            parameter.Value = value?.Name;
        }

        public override CategoryType Parse(object value) => Enumeration.FromName<CategoryType>(value as string);
    }
}
