using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas
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
