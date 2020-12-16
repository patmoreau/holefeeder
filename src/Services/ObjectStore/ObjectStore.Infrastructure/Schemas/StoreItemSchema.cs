using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure.Schemas
{
    [BsonIgnoreExtraElements]
    public class StoreItemSchema
    {
        public const string SCHEMA = "objectsData";

        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string MongoId { get; set; }
        
        [BsonElement("globalId")]
        public Guid Id { get; set; }

        [BsonElement("code")]
        public string Code { get; set; }

        [BsonElement("data")]
        public string Data { get; set; }

        [BsonElement("userId")]
        public Guid UserId { get; set; }
    }
}
