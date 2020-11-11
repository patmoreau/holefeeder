using System;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public interface IOwnedEntity
    {
        Guid UserId { get; }
    }
}
