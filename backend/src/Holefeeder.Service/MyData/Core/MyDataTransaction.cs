using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.MyData.Core;

internal sealed record MyDataTransaction(
    Id Id,
    DateOnly Date,
    Money Amount,
    string Description,
    TagList Tags,
    Id CategoryId,
    Id AccountId,
    Id? CashflowId,
    DateOnly? CashflowDate)
{
    public Result<MyDataTransaction> Validated() =>
        MyDataRecordErrors.Validated(this, (Date != default, MyDataRecordErrors.TransactionDateRequired));
}
