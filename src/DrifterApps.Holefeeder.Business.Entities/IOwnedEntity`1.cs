using System;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public interface IOwnedEntity<TEntity> : IOwnedEntity
    {
        TEntity WithUser(Guid userId);
    }
}
