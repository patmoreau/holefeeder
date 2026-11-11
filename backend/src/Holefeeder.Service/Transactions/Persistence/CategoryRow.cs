using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core;

namespace Holefeeder.Service.Transactions.Persistence;

internal sealed record CategoryRow(Guid Id, string Name, string Color, string Type, bool Favorite)
{
    public TransactionCategory ToCategory() =>
        new(Shared.Core.Id.Valid(Id), Name, Color, Enum.Parse<CategoryType>(Type, ignoreCase: true), Favorite);
}
