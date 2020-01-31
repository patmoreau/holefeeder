namespace DrifterApps.Holefeeder.Business.Entities
{
    public abstract class BaseEntity : IIdentityEntity
    {
        public string Id { get; }

        protected BaseEntity(string id)
        {
            Id = id;
        }
    }
}