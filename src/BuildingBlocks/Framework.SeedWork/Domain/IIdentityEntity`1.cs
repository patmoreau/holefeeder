namespace DrifterApps.Holefeeder.Framework.SeedWork.Domain
{
    public interface IIdentityEntity<TEntity> : IIdentityEntity where TEntity : Entity
    {
        TEntity WithId(string id);
    }
}
