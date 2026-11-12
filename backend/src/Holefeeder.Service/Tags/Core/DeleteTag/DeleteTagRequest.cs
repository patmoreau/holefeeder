using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Tags.Core.DeleteTag;

internal sealed record DeleteTagRequest(User User, TagName Tag);
