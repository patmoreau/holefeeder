using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas
{
    [BsonIgnoreExtraElements]
    public class AccountSchema : BaseSchema, IOwnedSchema
    {
        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("favorite")]
        public bool Favorite { get; set; }

        [BsonElement("openBalance"), BsonRepresentation(BsonType.Decimal128)]
        public decimal OpenBalance { get; set; }

        [BsonElement("openDate"), BsonDateTimeOptions(DateOnly = true)]
        public DateTime OpenDate { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("inactive")]
        public bool Inactive { get; set; }

        [BsonElement("guid")]
        public string Guid { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }
    }
}