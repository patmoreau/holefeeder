using System;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas
{
    [BsonIgnoreExtraElements]
    public class CategorySchema : BaseSchema, IOwnedSchema
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("type"), BsonRepresentation(BsonType.String), JsonConverter(typeof(JsonStringEnumConverter))]
        public CategoryType Type { get; set; }

        [BsonElement("color")]
        public string Color { get; set; }

        [BsonElement("budgetAmount"), BsonRepresentation(BsonType.Decimal128)]
        public decimal BudgetAmount { get; set; }

        [BsonElement("favorite")]
        public bool Favorite { get; set; }

        [BsonElement("system")]
        public bool System { get; set; }

        [BsonElement("guid")]
        public string GlobalId { get; set; }

        [BsonElement("userId")]
        public Guid UserId { get; set; }
    }
}
