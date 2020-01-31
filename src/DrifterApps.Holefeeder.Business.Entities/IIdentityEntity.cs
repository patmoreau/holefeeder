namespace DrifterApps.Holefeeder.Business.Entities
{
    public interface IIdentityEntity
    {
        string Id { get; }
    }

    public interface IIdentityEntity<TEntity> : IIdentityEntity
    {
        TEntity WithId(string id);
    }
}
