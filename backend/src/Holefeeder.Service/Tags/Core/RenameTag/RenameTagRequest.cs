using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Tags.Core.RenameTag;

internal sealed record RenameTagRequest(User User, TagName Tag, TagName NewTag);
