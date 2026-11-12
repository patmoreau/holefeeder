using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Cashflows.Core.ModifyCashflow;

internal sealed record ModifyCashflowRequest(User User, Id CashflowId, Money Amount, DateOnly EffectiveDate, string? Description, TagList Tags);
