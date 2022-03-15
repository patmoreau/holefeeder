using System;

using DrifterApps.Holefeeder.ObjectStore.Application;

namespace DrifterApps.Holefeeder.ObjectStore.API.Authorization;

public class RequestUserContext : IRequestUser
{
    public RequestUserContext(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}
