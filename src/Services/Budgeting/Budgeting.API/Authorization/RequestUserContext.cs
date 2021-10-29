using System;

using DrifterApps.Holefeeder.Budgeting.Application;

namespace DrifterApps.Holefeeder.Budgeting.API.Authorization
{
    public class RequestUserContext : IRequestUser
    {
        public Guid UserId { get; }

        public RequestUserContext(Guid userId)
        {
            UserId = userId;
        }
    }
}
