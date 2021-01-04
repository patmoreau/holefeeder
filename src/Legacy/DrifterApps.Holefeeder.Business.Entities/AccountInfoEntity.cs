namespace DrifterApps.Holefeeder.Business.Entities
{
    public class AccountInfoEntity : BaseEntity, IIdentityEntity<AccountInfoEntity>
    {
        public string Name { get; }

        public AccountInfoEntity(string id, string name) : base(id)
        {
            Name = name;
        }

        public AccountInfoEntity With(string id = null, string name = null) => new AccountInfoEntity(id ?? Id, name ?? Name);

        public AccountInfoEntity WithId(string id) => With(id);
    }
}
