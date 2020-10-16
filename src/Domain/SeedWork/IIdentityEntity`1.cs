namespace DrifterApps.Holefeeder.Domain.SeedWork
{
    public interface IIdentityEntity<TEntity> : IIdentityEntity where TEntity : Entity
    {
        TEntity WithId(string id);
    }
}
