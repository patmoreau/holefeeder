
using Holefeeder.Application.Context;

using IHttpUserContext = DrifterApps.Seeds.Application.IUserContext;

namespace Holefeeder.Application.UserContext;

internal class UserContext : IUserContext
{
    public Guid Id { get; }

    internal UserContext(IHttpUserContext userContext, BudgetingContext context)
    {
        Id = context.Users
            .Where(e => e.UserIdentities.Any(e => e.Sub == userContext.Id))
            .Select(e => e.Id)
            .Single();
    }

}
