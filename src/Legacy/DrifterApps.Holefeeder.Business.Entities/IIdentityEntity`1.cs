namespace DrifterApps.Holefeeder.Business.Entities
{
    public interface IIdentityEntity<TEntity> : IIdentityEntity
    {
        TEntity WithId(string id);
    }
}
