using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas
{
    [BsonIgnoreExtraElements]
    public class CashflowSchema : BaseSchema, IOwnedSchema
    {
        [BsonElement("effectiveDate"), BsonDateTimeOptions(DateOnly = true)]
        public DateTime EffectiveDate { get; set; }

        [BsonElement("amount"), BsonRepresentation(BsonType.Decimal128)]
        public decimal Amount { get; set; }

        [BsonElement("type")]
        public string IntervalType { get; set; }

        [BsonElement("frequency")]
        public int Frequency { get; set; }

        [BsonElement("recurrence")]
        public int Recurrence { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("account"), BsonRepresentation(BsonType.ObjectId)]
        public string Account { get; set; }

        [BsonElement("category"), BsonRepresentation(BsonType.ObjectId)]
        public string Category { get; set; }

        [BsonElement("inactive")]
        public bool Inactive { get; set; }

        [BsonElement("tags")]
        public IList<string> Tags { get; set; }

        [BsonElement("guid")]
        public string Guid { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }
    }
}
