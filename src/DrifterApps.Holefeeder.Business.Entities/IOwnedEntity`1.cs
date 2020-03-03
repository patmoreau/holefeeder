namespace DrifterApps.Holefeeder.Business.Entities
{
    public interface IOwnedEntity<TEntity> : IOwnedEntity
    {
        TEntity WithUser(string userId);
    }
}
