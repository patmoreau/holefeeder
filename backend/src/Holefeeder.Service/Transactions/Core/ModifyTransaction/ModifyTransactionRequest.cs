using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Transactions.Core.ModifyTransaction;

internal sealed record ModifyTransactionRequest(
    User User,
    Id Id,
    DateOnly Date,
    Money Amount,
    string? Description,
    Id AccountId,
    Id CategoryId,
    Tags Tags);
