using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.MyData.Core;

internal sealed record MyDataCategory(
    Id Id,
    CategoryType Type,
    string Name,
    string Color,
    Money BudgetAmount,
    bool Favorite,
    bool System,
    bool Inactive)
{
    private const int NameMaxLength = 255;

    public Result<MyDataCategory> Validated() =>
        MyDataRecordErrors.Validated(this,
            (!string.IsNullOrWhiteSpace(Name) && Name.Length <= NameMaxLength, MyDataRecordErrors.CategoryNameInvalid));
}
