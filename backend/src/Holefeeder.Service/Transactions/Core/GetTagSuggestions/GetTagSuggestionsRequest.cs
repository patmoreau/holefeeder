using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Transactions.Core.GetTagSuggestions;

internal sealed record GetTagSuggestionsRequest(User User, Id? CategoryId);
