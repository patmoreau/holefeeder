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
    bool Inactive);
