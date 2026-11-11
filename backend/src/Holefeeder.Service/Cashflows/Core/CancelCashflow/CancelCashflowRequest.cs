using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Cashflows.Core.CancelCashflow;

internal sealed record CancelCashflowRequest(User User, Id CashflowId);
