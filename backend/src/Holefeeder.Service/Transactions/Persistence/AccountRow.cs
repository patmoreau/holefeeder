using Holefeeder.Service.Transactions.Core;

namespace Holefeeder.Service.Transactions.Persistence;

internal sealed record AccountRow(Guid Id, string Name, bool Favorite)
{
    public TransactionAccount ToAccount() => new(Shared.Core.Id.Valid(Id), Name, Favorite);
}
