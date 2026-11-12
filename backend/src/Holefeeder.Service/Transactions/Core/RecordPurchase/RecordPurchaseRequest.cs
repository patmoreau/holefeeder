using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Transactions.Core.RecordPurchase;

internal sealed record RecordPurchaseRequest(
    User User,
    DateOnly Date,
    Money Amount,
    string Description,
    Id AccountId,
    Id CategoryId,
    TagList Tags,
    Repeat? Repeat);
