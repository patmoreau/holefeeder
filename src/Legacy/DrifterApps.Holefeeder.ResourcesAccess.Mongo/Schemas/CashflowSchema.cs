using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;
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

        [BsonElement("type"), BsonRepresentation(BsonType.String), JsonConverter(typeof(JsonStringEnumConverter))]
        public DateIntervalType IntervalType { get; set; }

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "MongoDB needs to assign it")]
        public IList<string> Tags { get; set; }

        [BsonElement("userId")]
        public Guid UserId { get; set; }
    }
}
