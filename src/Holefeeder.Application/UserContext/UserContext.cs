using DrifterApps.Seeds.Application;

using Holefeeder.Application.Context;

namespace Holefeeder.Application.UserContext;

internal class UserContext(IHttpUserContext userContext, BudgetingContext context) : IUserContext
{
    public Guid Id { get; } = context.Users
        .Where(user =>
            user.UserIdentities.Any(identity => identity.IdentityObjectId == userContext.IdentityObjectId))
        .Select(user => user.Id)
        .SingleOrDefault();
}
