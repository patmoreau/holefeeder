using DrifterApps.Seeds.Application;

using Holefeeder.Application.Context;

namespace Holefeeder.Application.UserContext;

internal class UserContext : IUserContext
{
    public Guid Id { get; }

    internal UserContext(IHttpUserContext userContext, BudgetingContext context)
    {
        Id = context.Users
            .Where(user =>
                user.UserIdentities.Any(identity => identity.IdentityObjectId == userContext.IdentityObjectId))
            .Select(user => user.Id)
            .Single();
    }
}
