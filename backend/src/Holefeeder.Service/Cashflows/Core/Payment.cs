using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Cashflows.Core;

internal sealed record Payment(
    Id Id,
    DateOnly Date,
    Money Amount,
    string Description,
    Id AccountId,
    Id CategoryId,
    Id CashflowId,
    DateOnly CashflowDate,
    string Tags);
