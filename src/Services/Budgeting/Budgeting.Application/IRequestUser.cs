using System;

namespace DrifterApps.Holefeeder.Budgeting.Application;

public interface IRequestUser
{
    Guid UserId { get; }
}
