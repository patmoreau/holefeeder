namespace DrifterApps.Holefeeder.Business.Entities
{
    public class ObjectDataEntity : BaseEntity, IIdentityEntity<ObjectDataEntity>, IOwnedEntity<ObjectDataEntity>
    {
        public string Code { get; }
        public string Data { get; }
        public string UserId { get; }

        public ObjectDataEntity(string id, string code, string data, string userId = "") : base(id)
        {
            Code = code;
            Data = data;
            UserId = userId;
        }

        public ObjectDataEntity With(string id = null, string code = null, string data = null, string userId = null) =>
            new ObjectDataEntity(id ?? Id, code ?? Code, data ?? Data, userId ?? UserId);

        public ObjectDataEntity WithId(string id) => this.With(id);

        public ObjectDataEntity WithUser(string userId) => this.With(userId: UserId);
    }
}
