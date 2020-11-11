using System;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas
{
    [BsonIgnoreExtraElements]
    public class AccountSchema : BaseSchema, IOwnedSchema
    {
        [BsonElement("type"), BsonRepresentation(BsonType.String), JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountType Type { get; set; }

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
        public string GlobalId { get; set; }

        [BsonElement("userId")]
        public Guid UserId { get; set; }
    }
}
