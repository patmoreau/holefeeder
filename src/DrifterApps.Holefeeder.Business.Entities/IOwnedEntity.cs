namespace DrifterApps.Holefeeder.Business.Entities
{
    public interface IOwnedEntity
    {
        string UserId { get; }
    }

    public interface IOwnedEntity<TEntity> : IOwnedEntity
    {
        TEntity WithUser(string userId);
    }
}
