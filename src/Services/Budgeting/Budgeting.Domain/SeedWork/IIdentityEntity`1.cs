namespace DrifterApps.Holefeeder.Budgeting.Domain.SeedWork
{
    public interface IIdentityEntity<TEntity> : IIdentityEntity where TEntity : Entity
    {
        TEntity WithId(string id);
    }
}
