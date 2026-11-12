using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.MyData.Core;

internal sealed record MyDataAccount(
    Id Id,
    AccountType Type,
    string Name,
    Money OpenBalance,
    DateOnly OpenDate,
    string Description,
    bool Favorite,
    bool Inactive)
{
    private const int NameMaxLength = 100;

    public Result<MyDataAccount> Validated() =>
        MyDataRecordErrors.Validated(this,
            (!string.IsNullOrWhiteSpace(Name) && Name.Length <= NameMaxLength, MyDataRecordErrors.AccountNameInvalid),
            (OpenDate != default, MyDataRecordErrors.AccountOpenDateRequired));
}
