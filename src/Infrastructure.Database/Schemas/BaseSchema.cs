using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Schemas
{
    public abstract class BaseSchema
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string MongoId { get; set; }
        
        [BsonElement("globalId")]
        public Guid Id { get; set; }

        [BsonExtraElements]
        public BsonDocument CatchAll { get; set; }
    }
}
