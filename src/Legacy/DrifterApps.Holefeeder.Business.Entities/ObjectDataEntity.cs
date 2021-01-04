using System;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class ObjectDataEntity : BaseEntity, IIdentityEntity<ObjectDataEntity>, IOwnedEntity<ObjectDataEntity>
    {
        public string Code { get; }
        public string Data { get; }
        public Guid UserId { get; }

        public ObjectDataEntity(string id, string code, string data, Guid userId = default) : base(id)
        {
            Code = code;
            Data = data;
            UserId = userId;
        }

        public ObjectDataEntity With(string id = null, string code = null, string data = null, Guid userId = default) =>
            new ObjectDataEntity(id ?? Id, code ?? Code, data ?? Data, userId == default ? UserId : userId);

        public ObjectDataEntity WithId(string id) => this.With(id);

        public ObjectDataEntity WithUser(Guid userId) => this.With(userId: userId);
    }
}
