using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Categories.Core;

internal sealed record Category(
    Id Id, CategoryName Name, CategoryType Type, CategoryColor Color, Money Budget, bool Favorite, bool System);
