using Holefeeder.Service.Support.Core;

using CoreId = Holefeeder.Service.Shared.Core.Id;
using CoreIdentityObjectId = Holefeeder.Service.Support.Core.IdentityObjectId;

namespace Holefeeder.Service.Support.Persistence;

internal record UserRow(Guid Id, string IdentityObjectId)
{
    public User ToUser() => User.Valid(CoreId.Valid(Id), CoreIdentityObjectId.Valid(IdentityObjectId));
}
