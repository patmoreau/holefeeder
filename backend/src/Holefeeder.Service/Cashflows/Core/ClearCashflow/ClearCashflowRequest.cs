using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Cashflows.Core.ClearCashflow;

internal sealed record ClearCashflowRequest(User User, Id CashflowId, DateOnly CashflowDate);
