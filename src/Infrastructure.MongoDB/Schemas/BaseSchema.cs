using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB.Schemas
{
    public abstract class BaseSchema
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string MongoId { get; set; }
        
        [BsonElement("guid")]
        public Guid Id { get; set; }

        [BsonExtraElements]
        public BsonDocument CatchAll { get; set; }
    }
}
