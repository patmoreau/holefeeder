using System;
using DrifterApps.Holefeeder.Domain.Enumerations;
using DrifterApps.Holefeeder.Domain.SeedWork;
using MongoDB.Bson.Serialization;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Serializers
{
    public class AccountTypeSerializer : IBsonSerializer<AccountType>
    {
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, AccountType value)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            
            context.Writer.WriteString(value?.Name);
        }

        public AccountType Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var type = context.Reader.ReadString();
            return Enumeration.FromName<AccountType>(type);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, value as AccountType);
        }

        public Type ValueType => typeof(AccountType);
    }
}
