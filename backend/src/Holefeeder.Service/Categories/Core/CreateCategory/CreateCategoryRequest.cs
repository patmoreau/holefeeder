using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Categories.Core.CreateCategory;

internal sealed record CreateCategoryRequest(
    User User, CategoryName Name, CategoryType Type, CategoryColor Color, Money Budget, bool Favorite);
