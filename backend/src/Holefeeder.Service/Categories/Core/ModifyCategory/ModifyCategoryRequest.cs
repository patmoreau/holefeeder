using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Categories.Core.ModifyCategory;

internal sealed record ModifyCategoryRequest(
    User User, Id Id, CategoryName Name, CategoryType Type, CategoryColor Color, Money Budget, bool Favorite);
