using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas
{
    [BsonIgnoreExtraElements]
    public class AccountDetailSchema : BaseSchema
    {
        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("transactionCount")]
        public int TransactionCount { get; set; }

        [BsonElement("balance"), BsonRepresentation(BsonType.Decimal128)]
        public decimal Balance { get; set; }

        [BsonElement("updated"), BsonDateTimeOptions(DateOnly = true)]
        public DateTime? Updated { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("favorite")]
        public bool Favorite { get; set; }

        [BsonElement("inactive")]
        public bool Inactive { get; set; }
    }
}