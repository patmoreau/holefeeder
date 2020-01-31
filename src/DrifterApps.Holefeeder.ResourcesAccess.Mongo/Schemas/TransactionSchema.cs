using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas
{
    [BsonIgnoreExtraElements]
    public class TransactionSchema : BaseSchema, IOwnedSchema
    {
        [BsonElement("date"), BsonDateTimeOptions(DateOnly = true)]
        public DateTime Date { get; set; }

        [BsonElement("amount"), BsonRepresentation(BsonType.Decimal128)]
        public decimal Amount { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("account"), BsonRepresentation(BsonType.ObjectId)]
        public string Account { get; set; }

        [BsonElement("category"), BsonRepresentation(BsonType.ObjectId)]
        public string Category { get; set; }

        [BsonElement("cashflow"), BsonRepresentation(BsonType.ObjectId)]
        public string Cashflow { get; set; }

        [BsonElement("cashflowDate"), BsonDateTimeOptions(DateOnly = true)]
        public DateTime? CashflowDate { get; set; }

        [BsonElement("tags")]
        public IList<string> Tags { get; set; }

        [BsonElement("guid")]
        public string Guid { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }
    }
}
