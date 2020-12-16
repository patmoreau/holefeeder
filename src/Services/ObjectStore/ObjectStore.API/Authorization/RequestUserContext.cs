using System;
using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;

namespace DrifterApps.Holefeeder.ObjectStore.API.Authorization
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
