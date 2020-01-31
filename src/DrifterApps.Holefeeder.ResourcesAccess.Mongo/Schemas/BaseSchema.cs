using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas
{
    public abstract class BaseSchema
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonExtraElements]
        public BsonDocument CatchAll { get; set; }
    }
}