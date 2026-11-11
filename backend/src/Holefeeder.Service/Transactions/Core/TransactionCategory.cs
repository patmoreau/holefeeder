using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Transactions.Core;

internal sealed record TransactionCategory(Id Id, string Name, string Color, CategoryType Type, bool Favorite);
