using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Cashflows.Core.PayCashflow;

internal sealed record PayCashflowRequest(
    User User,
    Id CashflowId,
    DateOnly Date,
    Money Amount,
    DateOnly CashflowDate,
    bool UpdateRecurringAmount);
