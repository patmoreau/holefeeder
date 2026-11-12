using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Users.Core;

internal sealed record RegisteredUser(Id Id, IdentityObjectId IdentityObjectId, IReadOnlyList<SystemCategory> Categories)
{
    public static RegisteredUser Register(IdentityObjectId identityObjectId) =>
        new(Id.New(), identityObjectId,
        [
            new SystemCategory(Id.New(), CategoryType.Expense, SystemCategory.TransferOutName),
            new SystemCategory(Id.New(), CategoryType.Gain, SystemCategory.TransferInName)
        ]);
}
