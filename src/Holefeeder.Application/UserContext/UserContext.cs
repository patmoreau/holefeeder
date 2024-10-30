using DrifterApps.Seeds.Application;

using Holefeeder.Application.Context;
using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Application.UserContext;

internal class UserContext(IHttpUserContext userContext, BudgetingContext context) : IUserContext
{
    public UserId Id => context.Users
        .Where(user =>
            user.UserIdentities.Any(identity => identity.IdentityObjectId == userContext.IdentityObjectId))
        .Select(user => user.Id)
        .SingleOrDefault() ?? UserId.Empty;
}
