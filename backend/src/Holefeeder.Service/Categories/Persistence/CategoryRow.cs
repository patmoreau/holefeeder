using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Categories.Persistence;

internal sealed record CategoryRow(Guid Id, string Name, string Type, string Color, decimal BudgetAmount, bool Favorite, bool System)
{
    public Category ToCategory() =>
        new(Shared.Core.Id.Valid(Id), CategoryName.Valid(Name), Enum.Parse<CategoryType>(Type, ignoreCase: true),
            CategoryColor.Valid(Color), Money.Valid(BudgetAmount), Favorite, System);
}
