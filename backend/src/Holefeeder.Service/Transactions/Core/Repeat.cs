using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Transactions.Core;

internal sealed record Repeat(DateOnly StartDate, IntervalType IntervalType, Frequency Frequency);
