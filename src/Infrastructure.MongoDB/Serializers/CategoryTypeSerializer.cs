using System;
using DrifterApps.Holefeeder.Domain.AggregatesModel.AccountAggregate;
using DrifterApps.Holefeeder.Domain.SeedWork;
using MongoDB.Bson.Serialization;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB.Serializers
{
    public class CategoryTypeSerializer : IBsonSerializer<CategoryType>
    {
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CategoryType value)
        {
            context.Writer.WriteString(value.Name);
        }

        public CategoryType Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var type = context.Reader.ReadString();
            return Enumeration.FromName<CategoryType>(type);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, value as CategoryType);
        }

        public Type ValueType => typeof(CategoryType);
    }
}
