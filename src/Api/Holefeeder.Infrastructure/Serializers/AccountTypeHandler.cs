using System.Data;

using Dapper;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Infrastructure.Serializers;

public class AccountTypeHandler : SqlMapper.TypeHandler<AccountType>
{
    public override void SetValue(IDbDataParameter parameter, AccountType value)
    {
        parameter.Value = value?.Name;
    }

    public override AccountType Parse(object value)
    {
        return Enumeration.FromName<AccountType>(value as string ?? string.Empty);
    }
}
