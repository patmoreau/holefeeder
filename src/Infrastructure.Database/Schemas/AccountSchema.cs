using System;
using DrifterApps.Holefeeder.Domain.Enumerations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Schemas
{
    [BsonIgnoreExtraElements]
    public class AccountSchema : BaseSchema, IOwnedSchema
    {
        public const string SCHEMA = "accounts";
        
        [BsonElement("type")]
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

        [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
    }
}
