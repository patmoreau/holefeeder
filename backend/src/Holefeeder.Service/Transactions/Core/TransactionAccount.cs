using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Transactions.Core;

internal sealed record TransactionAccount(Id Id, string Name, bool Favorite);
