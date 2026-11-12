using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Categories.Core.DeleteCategory;

internal sealed record DeleteCategoryRequest(User User, Id Id);
