using System;
using DrifterApps.Holefeeder.Domain.Enumerations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Schemas
{
    [BsonIgnoreExtraElements]
    public class CategorySchema : BaseSchema, IOwnedSchema
    {
        public const string SCHEMA = "categories";
        
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("type")]
        public CategoryType Type { get; set; }

        [BsonElement("color")]
        public string Color { get; set; }

        [BsonElement("budgetAmount"), BsonRepresentation(BsonType.Decimal128)]
        public decimal BudgetAmount { get; set; }

        [BsonElement("favorite")]
        public bool Favorite { get; set; }

        [BsonElement("system")]
        public bool System { get; set; }

        [BsonElement("userId"), BsonSerializer(typeof(GuidSerializer))]
        public Guid UserId { get; set; }
    }
}
