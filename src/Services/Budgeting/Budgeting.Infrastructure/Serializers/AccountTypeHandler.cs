using System.Data;

using Dapper;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Serializers;

public class AccountTypeHandler : SqlMapper.TypeHandler<AccountType>
{
    public override void SetValue(IDbDataParameter parameter, AccountType value)
    {
        parameter.Value = value?.Name;
    }

    public override AccountType Parse(object value) =>
        Enumeration.FromName<AccountType>(value as string ?? string.Empty);
}
