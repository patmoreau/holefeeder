using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Transactions.Core;

internal sealed record TagUsage(string Tag, Id CategoryId, int Count);
